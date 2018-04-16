using System;

namespace NHibernate.Validator.Util
{
	/// <summary>
	/// Specify if constraint attribute can be used multiple times with intersecting tags. (No tags, or same tags)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class AllowsMultipleWithIntersectingTagsAttribute : Attribute
	{
	}
}
