/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Edison.Engine.Repositories.Interfaces;
using Edison.Framework;
using Edison.Injector;
using System;
using Edison.Framework.Enums;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IReflectionRepository))]
    public class ReflectionRepository : IReflectionRepository
    {

        #region Constants

        private readonly IEnumerable<TestCaseAttribute> EmptyTestCase = new List<TestCaseAttribute>() { new TestCaseAttribute() };

        #endregion

        #region MemberInfo Calls

        public int GetRepeatValue(MemberInfo member)
        {
            var attr = member.GetCustomAttribute<RepeatAttribute>();
            return attr == default(RepeatAttribute)
                ? -1
                : attr.Value;
        }

        public IEnumerable<string> GetAuthors(MemberInfo member)
        {
            var authorAttributes = member.GetCustomAttributes<AuthorAttribute>();

            return authorAttributes == default(IEnumerable<AuthorAttribute>) || !authorAttributes.Any()
                ? new List<string>()
                : authorAttributes.Select(a => a.Name);
        }

        public string GetFullNamespace(MemberInfo member)
        {
            return member.DeclaringType.FullName + "." + member.Name;
        }

        public string GetNamespace(MemberInfo member)
        {
            return member.DeclaringType.FullName;
        }

        public IEnumerable<TestCaseAttribute> GetTestCases(MemberInfo member)
        {
            var cases = member.GetCustomAttributes<TestCaseAttribute>();

            return cases.Any()
                ? cases
                : EmptyTestCase;
        }

        public string GetVersion(MemberInfo member)
        {
            var versionAttribute = member.GetCustomAttributes().OfType<VersionAttribute>();

            return versionAttribute == default(IEnumerable<VersionAttribute>) || !versionAttribute.Any()
                ? string.Empty
                : versionAttribute.First().Value;
        }

        public IEnumerable<string> GetCategories(MemberInfo member)
        {
            return member.GetCustomAttributes<CategoryAttribute>().Select(x => x.Name);
        }

        public bool HasValidAttributes<T>(MemberInfo member, IList<string> includedCategories, IList<string> excludedCategories, string suite) where T : Attribute
        {
            var attributes = member.GetCustomAttributes();

            return member.GetCustomAttribute<T>() != default(T)
                && member.GetCustomAttribute<IgnoreAttribute>() == default(IgnoreAttribute)
                && HasValidCategories(member, includedCategories, excludedCategories)
                && HasValidSuite(member, suite);
        }

        public bool HasValidSuite(MemberInfo member, string suite)
        {
            if (string.IsNullOrWhiteSpace(suite))
            {
                return true;
            }

            var suiteAttr = member.GetCustomAttribute<SuiteAttribute>();
            return suiteAttr != default(SuiteAttribute) && suiteAttr.Name.Equals(suite, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool HasValidCategories(MemberInfo member, IList<string> includedCategories, IList<string> excludedCategories)
        {
            if (includedCategories == default(List<string>) && excludedCategories == default(List<string>))
            {
                return true;
            }

            var categories = member.GetCustomAttributes<CategoryAttribute>();
            var isTestFixture = member.GetCustomAttribute<TestFixtureAttribute>() != default(TestFixtureAttribute);

            if (isTestFixture && !categories.Any())
            {
                return true;
            }

            if (includedCategories != default(List<string>) && categories.Any(c => includedCategories.Any(i => i.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return true;
            }

            if (excludedCategories != default(List<string>) && categories.Any(c => excludedCategories.Any(e => e.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return false;
            }

            if (isTestFixture)
            {
                return true;
            }

            if (includedCategories != default(List<string>) && includedCategories.Any())
            {
                return false;
            }

            return true;
        }

        public bool HasValidConcurrency(MemberInfo member, ConcurrencyType concurrencyType, ConcurrencyType defaultConcurreny = ConcurrencyType.Parallel)
        {
            var concurrency = member.GetCustomAttribute<ConcurrencyAttribute>();

            return concurrency == default(ConcurrencyAttribute)
                ? concurrencyType == defaultConcurreny
                : concurrency.ConcurrencyType == concurrencyType;
        }

        #endregion

        #region Method Calls

        public ExpectedExceptionAttribute GetExpectedException(MethodInfo method)
        {
            var exceptionAttribute = method.GetCustomAttributes().OfType<ExpectedExceptionAttribute>();

            return exceptionAttribute == default(IEnumerable<ExpectedExceptionAttribute>) || !exceptionAttribute.Any()
                ? default(ExpectedExceptionAttribute)
                : exceptionAttribute.First();
        }

        public bool HasParameters(MethodInfo method)
        {
            var parameters = GetParameters(method);
            return parameters != default(ParameterInfo[]) && parameters.Any();
        }

        public ParameterInfo[] GetParameters(MethodInfo method)
        {
            return method.GetParameters();
        }

        public void Invoke(MethodInfo method, object activator, params object[] parameters)
        {
            if (method != default(MethodInfo))
            {
                if (HasParameters(method))
                {
                    method.Invoke(activator, parameters);
                }
                else
                {
                    method.Invoke(activator, default(object[]));
                }
            }
        }

        public void Invoke(IEnumerable<MethodInfo> methods, object activator, params object[] parameters)
        {
            if (methods != default(IEnumerable<MethodInfo>))
            {
                foreach (var method in methods)
                {
                    Invoke(method, activator, parameters);
                }
            }
        }

        #endregion

        #region Class Calls

        public IEnumerable<MethodInfo> GetMethods<T>(Type type,
            IList<string> includedCategories = default(List<string>),
            IList<string> excludedCategories = default(List<string>),
            IList<string> tests = default(List<string>),
            string suite = null) where T : Attribute
        {
            return type
                .GetMethods()
                .Where(t => HasValidAttributes<T>(t, includedCategories, excludedCategories, suite))
                .Where(t => tests == default(IList<string>) || !tests.Any() || tests.Contains(GetFullNamespace(t)));
        }
        
        public IEnumerable<string> GetSuites(Type type)
        {
            return type.GetCustomAttributes<SuiteAttribute>().Select(x => x.Name);
        }

        #endregion

    }
}
