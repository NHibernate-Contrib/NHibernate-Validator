using System.Reflection;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Engine;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Validator.Tests.Specifics.NHV119
{

	[TestFixture]
	public class Fixture : BaseValidatorFixture
	{
		[Test]
		public void TestSameAttributeByDifferentTagsAttributeConfig()
		{
			//var vl = GetClassValidator(typeof(Model1));

			var vtor = new ValidatorEngine();

			var m = new Model1();

			m.Qnt = 1000;
			vtor.Validate(m).Should().Be.Empty();

			m.Qnt = 50;
			vtor.Validate(m).Should().Not.Be.Empty();

			m.Qnt = 100;
			vtor.Validate(m, "T2").Should().Be.Empty();

			vtor.Validate(m, "T2", null).Should().Not.Be.Empty();

			m.Qnt = 10;
			vtor.Validate(m, "T1").Should().Be.Empty();
		}

		[Test]
		public void TestSameAttributeByDifferentTagsXmlOverAttributeConfig()
		{
			//var vl = GetClassValidator(typeof(Model1));

			var vtor = new ValidatorEngine();
			var cfg = new XmlConfiguration();
			cfg.Properties[Environment.ValidatorMode] = "OverrideAttributeWithExternal";
			string an = Assembly.GetExecutingAssembly().FullName;
			cfg.Mappings.Add(new MappingConfiguration(an, "NHibernate.Validator.Tests.Specifics.NHV119.Mappings.nhv.xml"));
			vtor.Configure(cfg);

			var m = new Model1();

			m.Qnt = 1000;
			vtor.Validate(m).Should().Be.Empty();

			m.Qnt = 100;
			vtor.Validate(m).Should().Not.Be.Empty();

			m.Qnt = 20;
			vtor.Validate(m, "T1").Should().Be.Empty();

			m.Qnt = 19;
			vtor.Validate(m, "T1").Should().Not.Be.Empty();


			m.Qnt = 199;
			vtor.Validate(m, "T2").Should().Not.Be.Empty();

			m.Qnt = 200;
			vtor.Validate(m, "T2").Should().Be.Empty();
		}

		[Test]
		public void TestSameAttributeByDifferentTagsFluentConfig()
		{
			//var vl = GetClassValidator(typeof(Model1));


			var configure = new FluentConfiguration();
			configure.Register(new[] { typeof(Model1Validation) })
			         .SetDefaultValidatorMode(ValidatorMode.UseExternal);

			var vtor = new ValidatorEngine();
			vtor.Configure(configure);

			var m = new Model1();

			m.Qnt = 3000;
			vtor.Validate(m).Should().Be.Empty();

			m.Qnt = 300;
			vtor.Validate(m).Should().Not.Be.Empty();

			m.Qnt = 30;
			vtor.Validate(m, "T1").Should().Be.Empty();


			m.Qnt = 299;
			vtor.Validate(m, "T2").Should().Not.Be.Empty();

			m.Qnt = 3001;
			vtor.Validate(m, "T2").Should().Be.Empty();
		}
	}

}
