using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Validator.Cfg.Loquacious;

namespace NHibernate.Validator.Tests.Specifics.NHV119
{
	public class Model1Validation : ValidationDef<Model1>
	{
		public Model1Validation()
		{
			Define(x => x.Qnt).GreaterThanOrEqualTo(30).WithTags("T1");
			Define(x => x.Qnt).GreaterThanOrEqualTo(300).WithTags("T2");
			Define(x => x.Qnt).GreaterThanOrEqualTo(3000).WithTags("T2"); //This definition must override previous one
		}
	}
}
