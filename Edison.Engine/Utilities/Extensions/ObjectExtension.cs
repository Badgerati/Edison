/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using YamlDotNet.Serialization;
using System.Linq;
using System;

namespace Edison.Engine.Utilities.Extensions
{
    public static class ObjectExtension
    {

        public static void InjectPropertyDefaultValues(this object obj)
        {
            var props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                var dv = prop.GetCustomAttribute<DefaultValueAttribute>();
                if (dv != default(DefaultValueAttribute))
                {
                    prop.SetValue(obj, dv.Value);
                }
            }
        }

        public static void InjectPropertyYamlValues(this object obj, Dictionary<object, object> yaml)
        {
            var keys = yaml.Keys.ToList();
            var props = obj.GetType().GetProperties();

            foreach (var prop in props)
            {
                var ignore = prop.GetCustomAttribute<YamlIgnoreAttribute>();
                if (ignore != default(YamlIgnoreAttribute))
                {
                    continue;
                }

                var member = prop.GetCustomAttribute<YamlMemberAttribute>();
                if (member != default(YamlMemberAttribute) && keys.Contains(member.Alias))
                {
                    var propType = prop.PropertyType;
                    var propTypeName = propType.Name.ToLowerInvariant();

                    if (propTypeName.Equals("ilist`1"))
                    {
                        var data = (List<object>)yaml[member.Alias];
                        var type = prop.PropertyType.GenericTypeArguments[0];

                        switch (type.Name.ToLower())
                        {
                            case "string":
                                prop.SetValue(obj, data.Cast<string>().ToList());
                                break;

                            case "int":
                                prop.SetValue(obj, data.Cast<int>().ToList());
                                break;
                        }
                    }
                    else
                    {
                        var converter = TypeDescriptor.GetConverter(propType);
                        prop.SetValue(obj, converter.ConvertFrom(yaml[member.Alias]));
                    }
                }
            }

        }

    }
}
