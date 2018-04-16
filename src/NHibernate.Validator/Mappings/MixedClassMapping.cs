using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Util;

namespace NHibernate.Validator.Mappings
{
	public abstract class MixedClassMapping : AbstractClassMapping
	{
		protected virtual void InitializeMembers(HashSet<MemberInfo> lmembers, IClassMapping baseMap, IClassMapping alternativeMap)
		{
			MixMembersWith(lmembers, baseMap);
			MixMembersWith(lmembers, alternativeMap);
		}

		protected virtual void InitializeClassAttributes(IClassMapping baseMap, IClassMapping alternativeMap)
		{
			classAttributes = new List<Attribute>();
			CombineAttribute(baseMap.GetClassAttributes(), classAttributes);
			CombineAttribute(alternativeMap.GetClassAttributes(), classAttributes);
		}

		protected void MixMembersWith(HashSet<MemberInfo> lmembers, IClassMapping mapping)
		{
			foreach (var info in mapping.GetMembers())
			{
				lmembers.Add(info);
				var mas = mapping.GetMemberAttributes(info);
				if (mas == null)
					continue;

				if (!membersAttributesDictionary.TryGetValue(info, out var attrs))
				{
					membersAttributesDictionary[info] = new List<Attribute>(mas);
				}
				else
				{
					CombineAttribute(mas, attrs);
					membersAttributesDictionary[info] = attrs;
				}
			}
		}

		protected static void CombineAttribute(IEnumerable<Attribute> origin, List<Attribute> dest)
		{
			foreach (var ma in origin)
			{
				var founded = dest.FindAll(attribute => ma.TypeId.Equals(attribute.TypeId));

				if (founded.Count > 0)
				{
					if (!AttributeUtils.AttributeAllowsMultiple(ma))
					{
						dest.Remove(founded[0]);
					}
					else
					{
						if ((ma is ITagableRule matr) && !AttributeUtils.AttributeAllowsMultipleWithIntersectingTags(ma))
						{
							foreach (var fd in founded)
							{
								if (!(fd is ITagableRule fdtr))
									continue;

								if ((fdtr.TagCollection.Count == 0 && matr.TagCollection.Count == 0)
								    || fdtr.TagCollection.Intersect(matr.TagCollection).Any()
								)
								{
									dest.Remove(fd);
								}
							}
						}
					}
				}

				dest.Add(ma);
			}
		}
	}
}
