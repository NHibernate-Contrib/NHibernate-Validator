using NHibernate.Validator.Constraints;
using NHibernate.Validator.Util;
using NUnit.Framework;

namespace NHibernate.Validator.Tests.Utils
{
	[TestFixture]
	public class AttributeUtilsFixture
	{
		[Test]
		public void AttributeCanBeMultiplied()
		{
			var patternAttribute = new PatternAttribute();
			Assert.AreEqual(true, (AttributeUtils.AttributeAllowsMultiple(patternAttribute)));
		}

		[Test]
		public void AttributeCannotBeMultiplied()
		{
			var lenghtAttribute = new IBANAttribute();
			Assert.AreEqual(false, (AttributeUtils.AttributeAllowsMultiple(lenghtAttribute)));
		}
	}
}
