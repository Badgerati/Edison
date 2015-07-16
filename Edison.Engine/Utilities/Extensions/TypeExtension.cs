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
    public static class TypeExtension
    {

        public static IEnumerable<MethodInfo> GetMethods<T>(this Type fixture, List<string> includedCategories = default(List<string>), List<string> excludedCategories = default(List<string>))
        {
            return fixture == default(Type)
                ? new List<MethodInfo>()
                : fixture.GetMethods().Where(t => ReflectionHelper.HasValidAttributes<T>(t.GetCustomAttributes(), includedCategories, excludedCategories));
        }

    }
}
