/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Utilities.Helpers
{
    public static class ReflectionHelper
    {

        public static void Invoke(IList<MethodInfo> methods, object activator, bool parametersOptional = false, params object[] parameters)
        {
            if (methods != default(IList<MethodInfo>))
            {
                foreach (var method in methods)
                {
                    Invoke(method, activator, parametersOptional, parameters);
                }
            }
        }

        public static void Invoke(MethodInfo method, object activator, bool parametersOptional = false, params object[] parameters)
        {
            if (method != default(MethodInfo))
            {
                try
                {
                    method.Invoke(activator, parameters);
                }
                catch
                {
                    if (parametersOptional)
                    {
                        method.Invoke(activator, default(object[]));
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public static bool HasValidAttributes<T>(IEnumerable<Attribute> attributes, List<string> includedCategories, List<string> excludedCategories)
        {
            return attributes != default(IEnumerable<Attribute>)
                && attributes.OfType<T>().Any()
                && !attributes.OfType<IgnoreAttribute>().Any()
                && HasValidCategories(attributes, includedCategories, excludedCategories);
        }

        public static bool HasValidCategories(IEnumerable<Attribute> attributes, List<string> includedCategories, List<string> excludedCategories)
        {
            var categories = attributes.OfType<CategoryAttribute>();

            if (includedCategories != default(List<string>) && categories.Any(c => includedCategories.Any(i => i.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return true;
            }

            if (excludedCategories != default(List<string>) && categories.Any(c => excludedCategories.Any(e => e.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return false;
            }

            if (includedCategories != default(List<string>) && includedCategories.Any())
            {
                return false;
            }

            return true;
        }

    }
}
