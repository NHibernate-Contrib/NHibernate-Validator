using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using NHibernate.Util;
using NHibernate.Validator.Cfg.MappingSchema;
using NHibernate.Validator.Util;

namespace NHibernate.Validator.Mappings
{
	public class XmlClassMapping : AbstractClassMapping
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(XmlClassMapping));

		public XmlClassMapping(NhvmClass meta)
		{
			clazz =
				ReflectHelper.ClassForFullName(
					TypeNameParser.Parse(meta.name, meta.rootMapping.@namespace, meta.rootMapping.assembly).ToString());

			classAttributes = meta.attributename == null
			                  	? new List<Attribute>(0)
			                  	: new List<Attribute>(meta.attributename.Length);

			List<MemberInfo> lmembers = meta.property == null
			                            	? new List<MemberInfo>(0)
			                            	: new List<MemberInfo>(meta.property.Length);

			if (meta.attributename != null)
			{
				log.Debug("Looking for class attributes");
				foreach (string attributeName in meta.attributename)
				{
					log.Info("Attribute to look for = " + attributeName);
					Attribute classAttribute = RuleAttributeFactory.CreateAttributeFromClass(clazz, attributeName);
					if (classAttribute != null)
						classAttributes.Add(classAttribute);
				}
			}

			if (meta.property != null)
			{
				foreach (NhvmProperty property in meta.property)
				{
					MemberInfo currentMember = TypeUtils.GetPropertyOrField(clazz, property.name);

					if (currentMember == null)
					{
						log.Error(
							string.Format("Property or field \"{0}\" was not found in the class: \"{1}\" ", property.name, clazz.FullName));
						continue;
					}
					log.Info("Looking for rules for property : " + property.name);
					lmembers.Add(currentMember);

					// creation of member attributes
					foreach (object rule in property.Items)
					{
						Attribute thisAttribute =
							RuleAttributeFactory.CreateAttributeFromRule(rule, meta.rootMapping.assembly, meta.rootMapping.@namespace);

						if (thisAttribute != null)
						{
							log.Info(string.Format("Adding member {0} to dictionary with attribute {1}", currentMember.Name, thisAttribute));
							if (!membersAttributesDictionary.ContainsKey(currentMember))
								membersAttributesDictionary.Add(currentMember, new List<Attribute>());

							membersAttributesDictionary[currentMember].Add(thisAttribute);
						}
					}
				}
			}
			members = lmembers.ToArray();
		}
	}
}