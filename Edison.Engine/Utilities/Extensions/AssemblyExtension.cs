/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Utilities.Extensions
{
    public static class AssemblyExtension
    {

        public static IEnumerable<Type> GetTypes<T>(this Assembly assembly, List<string> includedCategories = default(List<string>), List<string> excludedCategories = default(List<string>))
        {
            return assembly == default(Assembly)
                ? new List<Type>()
                : assembly.GetTypes().Where(t => ReflectionHelper.HasValidAttributes<T>(t.GetCustomAttributes(), includedCategories, excludedCategories));
        }

    }
}
