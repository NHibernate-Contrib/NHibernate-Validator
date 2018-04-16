using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Validator.Constraints;

namespace NHibernate.Validator.Tests.Specifics.NHV119
{
	public class Model1
	{
		[Min(10, Tags = "T1")]
		[Min(777, Tags = "T2")] //This attribute will be ignored (redefined), by next one
		[Min(100, Tags = "T2")] //And this will be redefined by xml config in TestSameAttributeByDifferentTagsXmlOverAttributeConfig test
		[Min(1000)]
		public int Qnt { get; set; }
	}
}
