using System;
using System.Collections;
using System.Linq;

using NHibernate.Mapping;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Constraints;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Event;
using NHibernate.Validator.Exceptions;

using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Validator.Tests.Integration
{
	[TestFixture]
	class FromNhibMetadataFixture : PersistenceTest
	{

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"Integration.FromNhibMetadata.hbm.xml",
					};
			}
		}

		protected ISharedEngineProvider fortest;
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			// The ValidatorInitializer and the ValidateEventListener share the same engine

			// Initialize the SharedEngine
			fortest = new NHibernateSharedEngineProvider();
			Cfg.Environment.SharedEngineProvider = fortest;
			ValidatorEngine ve = Cfg.Environment.SharedEngineProvider.GetEngine();
			ve.Clear();
			XmlConfiguration nhvc = new XmlConfiguration();
			nhvc.Properties[Cfg.Environment.ApplyToDDL] = "true";
			nhvc.Properties[Cfg.Environment.AutoGenerateFromMapping] = "true";
			nhvc.Properties[Cfg.Environment.AutoregisterListeners] = "true";
			nhvc.Properties[Cfg.Environment.ValidatorMode] = "UseAttribute";
			nhvc.Properties[Cfg.Environment.MessageInterpolatorClass] = typeof(PrefixMessageInterpolator).AssemblyQualifiedName;
			
			ve.Configure(nhvc);
			//ve.IsValid(new HibernateAnnotationIntegrationFixture.AnyClass());// add the element to engine for test

			ValidatorInitializer.Initialize(configuration);
		}

		protected override void OnTestFixtureTearDown()
		{
			// reset the engine
			Cfg.Environment.SharedEngineProvider = null;
		}

		public void CleanupData()
		{
			ISession s = OpenSession();
			ITransaction txn = s.BeginTransaction();

			s.Delete("from FromNhibMetadata");

			txn.Commit();
			s.Close();
		}


		[Test]
		public void ApplyFromStringColumn()
		{
			var ve = Cfg.Environment.SharedEngineProvider.GetEngine();
			var vl = ve.GetValidator<FromNhibMetadata>();

			Assert.IsTrue(vl.HasValidationRules, "Validation rules must be created from NHib metadata");

			var sva = vl.GetMemberConstraints("StrValue").FirstOrDefault();

			Assert.IsInstanceOf<LengthAttribute>(sva, "LengthAttribute should be generated from Nhib metadata for StrValue property");

			var sval = sva as LengthAttribute;

			Assert.AreEqual(5, sval.Max);

		}

		[Test]
		public void ApplyFromDateNotNullColumn()
		{
			var ve = Cfg.Environment.SharedEngineProvider.GetEngine();
			var vl = ve.GetValidator<FromNhibMetadata>();

			var sva = vl.GetMemberConstraints("DateNotNull").FirstOrDefault();

			Assert.IsInstanceOf<NotNullAttribute>(sva, "NotNullAttribute should be generated from NHib metadata for DateNotNull property");

		}


		[Test]
		public void ApplyFromDecimalColumn()
		{
			var ve = Cfg.Environment.SharedEngineProvider.GetEngine();
			var vl = ve.GetValidator<FromNhibMetadata>();

			var sva = vl.GetMemberConstraints("Dec").FirstOrDefault();

			Assert.IsInstanceOf<DigitsAttribute>(sva, "DigitsAttribute should be generated from NHib metadata for Dec property");

			var svad = sva as DigitsAttribute;

			Assert.AreEqual(3, svad.IntegerDigits);
			Assert.AreEqual(2, svad.FractionalDigits);

		}

		[Test]
		public void ApplyFromEnumVColumn()
		{
			var ve = Cfg.Environment.SharedEngineProvider.GetEngine();
			var vl = ve.GetValidator<FromNhibMetadata>();

			var sva = vl.GetMemberConstraints("EnumV").FirstOrDefault();

			Assert.IsInstanceOf<EnumAttribute>(sva, "EnumAttribute should be generated from NHib metadata for EnumV property");

			var classMapping = cfg.GetClassMapping(typeof(FromNhibMetadata));
			IEnumerator ie = classMapping.GetProperty("EnumV").ColumnIterator.GetEnumerator();
			ie.MoveNext();
			var serialColumn = (Column)ie.Current;
			Assert.AreEqual("EnumV in (0, 1)", serialColumn.CheckConstraint, "Validator annotation should generate valid check for Enums");
		}

		[Test]
		public void ApplyFromComponentStringColumn()
		{
			var ve = Cfg.Environment.SharedEngineProvider.GetEngine();
			var vl = ve.GetValidator<FromNhibMetadata>();

			Assert.IsTrue(vl.HasValidationRules, "Validation rules must be created from NHib metadata");

			var sva = vl.GetMemberConstraints("Cmp").FirstOrDefault();

			Assert.IsInstanceOf<ValidAttribute>(sva, "ValidAttribute should be generated from NHib metadata for Component property");

			var vli = vl as IClassValidatorImplementor;
			vl = vli.ChildClassValidators[typeof(Cmp1)];

			sva = vl.GetMemberConstraints("CStrValue").FirstOrDefault();

			Assert.IsInstanceOf<LengthAttribute>(sva, "LengthAttribute should be generated from NHib metadata for Cmp1.CStrValue property");
			var sval = sva as LengthAttribute;

			Assert.AreEqual(3, sval.Max);

			vl = vli.ChildClassValidators[typeof(Cmp2)];

			sva = vl.GetMemberConstraints("CStrValue1").FirstOrDefault();

			Assert.IsInstanceOf<LengthAttribute>(sva, "LengthAttribute should be generated from NHib metadata for Cmp2.CStrValue1 property");
			sval = sva as LengthAttribute;

			Assert.AreEqual(5, sval.Max);
		}



		[Test]
		public void ApplyConstraintsOnEmbededeComponentsColumns()
		{
			var classMapping = cfg.GetClassMapping(typeof(FromNhibMetadata));
			var ie = classMapping.GetProperty("Cmp").ColumnIterator.GetEnumerator();
			ie.MoveNext();
			var col = (Column)ie.Current;
			while (col.Name != "CEnumV")
			{
				ie.MoveNext();
				col = (Column)ie.Current;
			}
			Assert.AreEqual("CEnumV in (0, 1)", col.CheckConstraint, "Validator annotation should generate valid check for CEnumV column (property of embedded component)");

			var prop = classMapping.GetProperty("Cmps2");
			var col1 = prop.Value as Mapping.Collection;
			var cmp = col1.Element as Component;

			ie = cmp.ColumnIterator.GetEnumerator();
			ie.MoveNext();
			col = (Column)ie.Current;
			while (col.Name != "CEnumV1")
			{
				ie.MoveNext();
				col = (Column)ie.Current;
			}
			Assert.AreEqual("CEnumV1 in (0, 1)", col.CheckConstraint, "Validator annotation should generate valid check for CEnumV1 column (property of embedded component in collection)");
		}


		[Test]
		public void Events()
		{
			ISession s;
			ITransaction tx;

			var x = new FromNhibMetadata();
			x.Id = 1;

			x.StrValue = "123456";
			x.DateNotNull = DateTime.Today;
			//x.DateNotNull = null; //!! NHib check not-null itself before integrated validators and throw exception
									//   Why it does not check the at еру same place Length, Precision And scale and so on, if all this known from it configuration? 
			x.Dec = 1234.567M;
			x.EnumV = (En1)42;


			s = OpenSession();
			tx = s.BeginTransaction();
			try
			{
				s.Save(x);
				tx.Commit();
				Assert.Fail("entity should have been validated");
			}
			catch (InvalidStateException e)
			{
				//success
				var invalidValues = e.GetInvalidValues();
				invalidValues.Should().Have.Count.EqualTo(3);
			}
			finally
			{
				if (tx != null && !tx.WasCommitted)
				{
					tx.Rollback();
				}
				s.Close();
			}


			x.DateNotNull = null; //But if we will call validators before SavingToNHib Null will be chacked
			var ve = Cfg.Environment.SharedEngineProvider.GetEngine();
			var ivals = ve.Validate(x);
			ivals.Should().Have.Count.EqualTo(4);


			// Don't throw exception if it is valid
			x = new FromNhibMetadata();
			x.Id = 2;

			x.StrValue = "12345";
			x.DateNotNull = DateTime.Today;
			x.Dec = 123.45M;
			x.EnumV = En1.v1;

			try
			{
				using (s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.Save(x);
					t.Commit();
				}
			}
			catch (InvalidStateException)
			{
				Assert.Fail("Valid entity cause InvalidStateException");
			}

			// Update check
			try
			{
				using (s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var saved = s.Get<FromNhibMetadata>(2);
					saved.StrValue = "123456";
					//saved.DateNotNull = null;
					saved.DateNotNull = DateTime.Now;
					saved.Dec = 5678.900M;
					saved.EnumV = (En1)66;

					s.Update(saved);
					t.Commit();
					Assert.Fail("entity should have been validated");
				}
			}
			catch (InvalidStateException e)
			{
				e.GetInvalidValues().Should().Have.Count.EqualTo(3);
			}

			try
			{
				using (s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var saved = s.Get<FromNhibMetadata>(2);
					saved.StrValue = "123";
					saved.DateNotNull = DateTime.Now;
					saved.Dec = 876.54M;
					saved.EnumV = En1.v2;

					s.Update(saved);
					t.Commit();
				}
			}
			catch (InvalidStateException)
			{
				Assert.Fail("Valid entity cause InvalidStateException");
			}

			// clean up
			using (s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var saved = s.Get<FromNhibMetadata>(2);
				s.Delete(saved);
				t.Commit();
			}

		}

		[Test]
		public void EventsComponent()
		{
			var x = new FromNhibMetadata();
			x.Id = 3;

			x.StrValue = "12345";
			x.DateNotNull = DateTime.Today;
			x.Dec = 123.45M;
			x.EnumV = En1.v1;

			x.Cmp = new Cmp1();
			x.Cmp.CEnumV = (En1)66;
			x.Cmp.CStrValue = "1234";
			x.Cmps2.Add(new Cmp2 { CEnumV1 = (En1)66, CStrValue1 = "12345XXXX" }); 

			var s = OpenSession();
			var tx = s.BeginTransaction();
			try
			{
				s.Save(x);
				tx.Commit();
				Assert.Fail("entity should have been validated");
			}
			catch (InvalidStateException e)
			{
				//success
				var invalidValues = e.GetInvalidValues();
				invalidValues.Should().Have.Count.EqualTo(4);
			}
			finally
			{
				if (tx != null && !tx.WasCommitted)
				{
					tx.Rollback();
				}
				s.Close();
			}

			x.Cmps2.Clear();
			x.Cmp.CEnumV = En1.v1;
			x.Cmp.CStrValue = "123";

			x.Cmps2.Add(new Cmp2 {CEnumV1 = En1.v2, CStrValue1 = "12345"}); 

			try
			{
				using (s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.Save(x);
					t.Commit();
				}
			}
			catch (InvalidStateException)
			{
				Assert.Fail("Valid entity cause InvalidStateException");
			}

			// clean up
			using (s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				var saved = s.Get<FromNhibMetadata>(3);
				s.Delete(saved);
				t.Commit();
			}


		}



	}
}
