/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.IO;
using Edison.Engine.Utilities.Helpers;
using Edison.Engine.Utilities.Extensions;
using Edison.Framework;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IAssemblyRepository))]
    public class AssemblyRepository : IAssemblyRepository
    {

        #region Repositories

        private IPathRepository PathRepository
        {
            get { return DIContainer.Instance.Get<IPathRepository>(); }
        }

        private IReflectionRepository ReflectionRepository
        {
            get { return DIContainer.Instance.Get<IReflectionRepository>(); }
        }

        #endregion

        public Assembly LoadFile(string path)
        {
            return Assembly.LoadFile(PathRepository.GetFullPath(path));
        }

        public Assembly LoadFrom(string assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile);
        }

        public Assembly GetEntryAssembly()
        {
            return Assembly.GetEntryAssembly();
        }

        public IEnumerable<MethodInfo> GetAllTests(Assembly assembly)
        {
            return GetTests(assembly, null, null, null, null);
        }

        public IOrderedEnumerable<Type> GetTestFixtures(Assembly assembly, IList<string> includedCategories, IList<string> excludedCategories, IList<string> fixtures, IList<string> tests)
        {
            return GetTypes<TestFixtureAttribute>(assembly, includedCategories, excludedCategories)
                .Where(t => fixtures == default(List<string>) || fixtures.Count == 0 || fixtures.Contains(t.FullName))
                .Where(x => ReflectionRepository.GetMethods<TestAttribute>(x, includedCategories, excludedCategories, tests).Any())
                .OrderBy(t => t.FullName);
        }

        public IEnumerable<MethodInfo> GetTests(Assembly assembly, IList<string> includedCategories, IList<string> excludedCategories, IList<string> fixtures, IList<string> tests)
        {
            var _fixtures = GetTestFixtures(assembly, includedCategories, excludedCategories, fixtures, tests);
            var _tests = new List<MethodInfo>(_fixtures.Count() * 30);

            foreach (var fixture in _fixtures)
            {
                _tests.AddRange(ReflectionRepository.GetMethods<TestAttribute>(fixture, includedCategories, excludedCategories, tests));
            }

            return _tests;
        }

        public IEnumerable<Type> GetTypes<T>(Assembly assembly, IList<string> includedCategories = null, IList<string> excludedCategories = null)
        {
            return assembly
                .GetTypes()
                .Where(t => ReflectionHelper.HasValidAttributes<T>(t.GetCustomAttributes(), includedCategories, excludedCategories));
        }

        public int GetTestCount(Assembly assembly, IList<string> includedCategories, IList<string> excludedCategories, IList<string> fixtures, IList<string> tests)
        {
            var _fixtures = GetTestFixtures(assembly, includedCategories, excludedCategories, fixtures, tests);
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

                var _tests = ReflectionRepository.GetMethods<TestAttribute>(fixture, includedCategories, excludedCategories, tests);

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
