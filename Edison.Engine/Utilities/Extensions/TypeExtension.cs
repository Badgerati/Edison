/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Utilities.Helpers;
using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Edison.Engine.Utilities.Extensions
{
    public static class TypeExtension
    {

        public static IEnumerable<MethodInfo> GetMethods<T>(
            this Type fixture,
            IList<string> includedCategories = default(List<string>),
            IList<string> excludedCategories = default(List<string>),
            IList<string> tests = default(List<string>))
        {
            return fixture
                .GetMethods()
                .Where(t => ReflectionHelper.HasValidAttributes<T>(t.GetCustomAttributes(), includedCategories, excludedCategories))
                .Where(t => tests == default(List<string>) || tests.Count == 0 || tests.Contains(t.GetFullNamespace()));
        }

        public static int GetRepeatValue(this Type type)
        {
            var attr = type.GetCustomAttribute<RepeatAttribute>();
            return attr == default(RepeatAttribute)
                ? -1
                : attr.Value;
        }

        public static IList<TestCaseAttribute> GetTestCases(this Type type)
        {
            var cases = type.GetCustomAttributes<TestCaseAttribute>().ToList();

            if (!cases.Any())
            {
                cases.Add(new TestCaseAttribute());
            }

            return cases;
        }

    }
}
