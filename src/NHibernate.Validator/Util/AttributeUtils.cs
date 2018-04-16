using System;
using System.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Validator.Util
{
	/// <summary>
	/// Utils metods for attributes
	/// </summary>
	public static class AttributeUtils
	{
		/// <summary>
		/// Returns true if the attribute can be declared more than one time for the same element
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static bool AttributeAllowsMultiple(Attribute attribute)
		{
			var usageAttribute = Attribute.GetCustomAttribute(attribute.GetType(), typeof(AttributeUsageAttribute));
			return ((AttributeUsageAttribute)usageAttribute).AllowMultiple;
		}

		/// <summary>
		/// Returns true if the attribute can be declared more than one time for the same element and this attribute marked with [AllowsMultipleWithSameTags]
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		public static bool AttributeAllowsMultipleWithIntersectingTags(Attribute attribute)
		{
			var attr = Attribute.GetCustomAttribute(attribute.GetType(), typeof(AllowsMultipleWithIntersectingTagsAttribute));
			return AttributeAllowsMultiple(attribute) && attr != null;
		}

		/// <summary>
		/// Return true if same attribute applied to property or field more then 1 time. It can happens when attribute used with different tags
		/// </summary>
		/// <param name="property"></param>
		/// <param name="attributeType"></param>
		/// <returns></returns>
		public static bool AttributeUsedMultipleTimesOnProperty(Property property, System.Type attributeType)
		{
			//Next happens for component properties. Not possible to access component class metadata back from Property in NHib 5.0
			//TODO: How to fix this?
			if (property.PersistentClass == null)
				return false;

			var tp = System.Type.GetType(((RootClass) property.PersistentClass).RootClazz.ClassName);
			var member = (MemberInfo) tp.GetProperty(property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (member == null)
				member = tp.GetField(property.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if (member == null)
				return false;

			var attributes = member.GetCustomAttributes(attributeType, true);
			return attributes.Length > 1;

		}


	}
}
