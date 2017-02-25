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

        public string GetAssemblyVersion()
        {
            return GetEntryAssembly().GetName().Version.ToString();
        }

        public IEnumerable<Type> GetTestFixtures(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests,
            string suite)
        {
            return GetTypes<TestFixtureAttribute>(assembly, includedCategories, excludedCategories, suite)
                .Where(t => fixtures == default(IList<string>) || !fixtures.Any() || fixtures.Contains(t.FullName))
                .Where(x => ReflectionRepository.GetMethods<TestAttribute>(x, includedCategories, excludedCategories, tests).Any())
                .OrderBy(t => t.FullName)
                .ToList();
        }

        public Tuple<IEnumerable<MethodInfo>, IEnumerable<Type>> GetTests(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests,
            string suite)
        {
            var _fixtures = GetTestFixtures(assembly, includedCategories, excludedCategories, fixtures, tests, suite);
            if (!_fixtures.Any())
            {
                return Tuple.Create(Enumerable.Empty<MethodInfo>(), _fixtures);
            }

            var _tests = _fixtures
                .Select(x => ReflectionRepository.GetMethods<TestAttribute>(x, includedCategories, excludedCategories, tests))
                .ToList();

            if (!_tests.Any())
            {
                Tuple.Create(Enumerable.Empty<MethodInfo>(), _fixtures);
            }

            var _aggrTests = _tests.Aggregate((a, b) => b.Concat(a));
            return Tuple.Create(_aggrTests, _fixtures);
        }

        public IEnumerable<string> GetSuites(
            Assembly assembly,
            IEnumerable<Type> fixtures = default(IEnumerable<Type>))
        {
            if (fixtures == default(IEnumerable<Type>))
            {
                fixtures = GetTestFixtures(assembly, default(IList<string>), default(IList<string>), default(IList<string>), default(IList<string>), null);
            }

            if (!fixtures.Any())
            {
                return Enumerable.Empty<string>();
            }

            return fixtures
                .Select(x => ReflectionRepository.GetSuites(x))
                .Aggregate((a, b) => b.Concat(a))
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public IEnumerable<string> GetCategories(
            Assembly assembly,
            IEnumerable<MethodInfo> tests = default(IEnumerable<MethodInfo>),
            IEnumerable<Type> fixtures = default(IEnumerable<Type>))
        {
            var items = default(Tuple<IEnumerable<MethodInfo>, IEnumerable<Type>>);
            if (tests == default(IEnumerable<MethodInfo>))
            {
                items = GetTests(assembly, default(IList<string>), default(IList<string>), default(IList<string>), default(IList<string>), null);
                tests = items.Item1;
            }

            if (fixtures == default(IEnumerable<Type>))
            {
                fixtures = items == default(Tuple<IEnumerable<MethodInfo>, IEnumerable<Type>>)
                    ? GetTestFixtures(assembly, default(IList<string>), default(IList<string>), default(IList<string>), default(IList<string>), null)
                    : items.Item2;
            }

            if (!fixtures.Any())
            {
                return Enumerable.Empty<string>();
            }

            var fixtureCategories = fixtures
                .Select(x => ReflectionRepository.GetCategories(x))
                .Aggregate((a, b) => b.Concat(a))
                .Distinct();

            var testCategories = tests
                .Select(x => ReflectionRepository.GetCategories(x))
                .Aggregate((a, b) => b.Concat(a))
                .Distinct();

            return fixtureCategories
                .Union(testCategories)
                .OrderBy(x => x)
                .ToList();
        }

        public IEnumerable<Type> GetTypes<T>(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            string suite) where T : Attribute
        {
            return assembly
                .GetTypes()
                .Where(t => ReflectionRepository.HasValidAttributes<T>(t, includedCategories, excludedCategories, suite));
        }

        public int GetTestCount(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests,
            string suite)
        {
            var _fixtures = GetTestFixtures(assembly, includedCategories, excludedCategories, fixtures, tests, suite);

            if (!_fixtures.Any())
            {
                return 0;
            }

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
