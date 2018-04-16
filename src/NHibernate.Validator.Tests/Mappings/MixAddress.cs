using NHibernate.Validator.Constraints;

namespace NHibernate.Validator.Tests.Mappings
{
	public class MixAddress
	{
		[NotNull]
		public static string blacklistedZipCode;

		[Length(Max = 20), NotNull]
		private string country;

		[Range(Min = -2, Max = 50, Message = "{floor.out.of.range} (escaping #{el})")] 
		public int floor;

		private long id;
		private bool internalValid = true;
		private string line1;
		private string line2;
		private string state;
		private string zip;

		[Min(1), Range(Max = 2000)]
		public long Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Country
		{
			get { return country; }
			set { country = value; }
		}

		[NotNull]
		public string Line1
		{
			get { return line1; }
			set { line1 = value; }
		}

		[Length(Max = 3), NotNull]
		public string State
		{
			get { return state; }
			set { state = value; }
		}

		[Pattern(Regex = "[0-9]+")]
		[NotNull]
		public string Zip
		{
			get { return zip; }
			set { zip = value; }
		}

		public string Line2
		{
			get { return line2; }
			set { line2 = value; }
		}

		[AssertTrue]
		public bool InternalValid
		{
			get { return internalValid; }
			set { internalValid = value; }
		}


		[Min(1, Tags = "T1")]    //This attribute will be redefined in xml mapping
		[Min(1000, Tags = "T2")]
		[Min(333, Tags = "T3")]  //And this also will be redefined by xml
		public int Num { get; set; }

	}
}
