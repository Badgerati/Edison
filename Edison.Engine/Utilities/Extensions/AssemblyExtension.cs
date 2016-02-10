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
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Utilities.Extensions
{
    public static class AssemblyExtension
    {

        public static IEnumerable<Type> GetTypes<T>(this Assembly assembly,
            List<string> includedCategories = default(List<string>),
            List<string> excludedCategories = default(List<string>))
        {
            return assembly
                .GetTypes()
                .Where(t => ReflectionHelper.HasValidAttributes<T>(t.GetCustomAttributes(), includedCategories, excludedCategories));
        }
        
        public static IOrderedEnumerable<Type> GetTestFixtures(this Assembly assembly, List<string> includedCategories,
            List<string> excludedCategories, List<string> fixtures)
        {
            return assembly
                .GetTypes<TestFixtureAttribute>(includedCategories, excludedCategories)
                .Where(t => fixtures == default(List<string>) || fixtures.Count == 0 || fixtures.Contains(t.FullName))
                .ToList()
                .OrderBy(t => t.FullName);
        }

        public static IEnumerable<MethodInfo> GetAllTests(this Assembly assembly)
        {
            return assembly.GetTests(null, null, null, null);
        }

        public static IEnumerable<MethodInfo> GetTests(this Assembly assembly, List<string> includedCategories,
            List<string> excludedCategories, List<string> fixtures, List<string> tests)
        {
            var _fixtures = assembly.GetTestFixtures(includedCategories, excludedCategories, fixtures);
            var _tests = new List<MethodInfo>(_fixtures.Count() * 30);

            foreach (var fixture in _fixtures)
            {
                _tests.AddRange(fixture.GetMethods<TestAttribute>(includedCategories, excludedCategories, tests));
            }

            return _tests;
        }

        public static int GetTestCount(this Assembly assembly, List<string> includedCategories,
            List<string> excludedCategories, List<string> fixtures, List<string> tests)
        {
            var _fixtures = assembly.GetTestFixtures(includedCategories, excludedCategories, fixtures);
            var _count = 0;
            var _fixtureCount = 0;
            var _testCount = 0;
            var _fixtureCaseCount = 0;
            var _fixtureRepeat = default(RepeatAttribute);
            var _testCaseCount = 0;
            var _testRepeat = default(RepeatAttribute);

            foreach (var fixture in _fixtures)
            {
                _testCount = 0;
                _fixtureCount = 0;
                _testCaseCount = 0;
                _fixtureCaseCount = 0;

                var _tests = fixture.GetMethods<TestAttribute>(includedCategories, excludedCategories, tests);

                foreach (var test in _tests)
                {
                    _testCaseCount = test.GetCustomAttributes<TestCaseAttribute>().Count();
                    _testCount = (_testCaseCount == 0 ? 1 : _testCaseCount);

                    _testRepeat = test.GetCustomAttribute<RepeatAttribute>();
                    _testCount *= (_testRepeat == default(RepeatAttribute) ? 1 : _testRepeat.Value);

                    _fixtureCount += _testCount;
                }

                _fixtureCaseCount = fixture.GetCustomAttributes<TestCaseAttribute>().Count();
                _fixtureCount *= (_fixtureCaseCount == 0 ? 1 : _fixtureCaseCount);

                _fixtureRepeat = fixture.GetCustomAttribute<RepeatAttribute>();
                _fixtureCount *= (_fixtureRepeat == default(RepeatAttribute) ? 1 : _fixtureRepeat.Value);

                _count += _fixtureCount;
            }

            return _count;
        }

    }
}
