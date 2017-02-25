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
using Edison.Engine.Utilities.Helpers;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IReflectionRepository))]
    public class ReflectionRepository : IReflectionRepository
    {

        #region Constants

        private readonly IEnumerable<TestCaseAttribute> EmptyTestCase = new List<TestCaseAttribute>() { new TestCaseAttribute() };
        private readonly RepeatAttribute SingleRepeat = new RepeatAttribute(1);

        #endregion

        #region MemberInfo Calls

        /// <summary>
        /// Determines whether or not this member is a type object
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool IsType(MemberInfo member)
        {
            return ((member as Type) != default(Type));
        }

        /// <summary>
        /// Determines whether or not this member is a method object
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public bool IsMethod(MemberInfo member)
        {
            return ((member as MethodInfo) != default(MethodInfo));
        }

        /// <summary>
        /// Gets the repeat value.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public RepeatAttribute GetRepeatValue(MemberInfo member)
        {
            return member.GetCustomAttribute<RepeatAttribute>() ?? SingleRepeat;
        }

        /// <summary>
        /// Gets the authors.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public IEnumerable<string> GetAuthors(MemberInfo member)
        {
            var authorAttributes = member.GetCustomAttributes<AuthorAttribute>();

            return authorAttributes == default(IEnumerable<AuthorAttribute>) || !authorAttributes.Any()
                ? new List<string>()
                : authorAttributes.Select(a => a.Name);
        }

        /// <summary>
        /// Gets the full namespace, which includes the normal namespace plus the method name.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public string GetFullNamespace(MemberInfo member)
        {
            return GetNamespace(member) + "." + member.Name;
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public string GetNamespace(MemberInfo member)
        {
            return member.DeclaringType == default(Type)
                ? ((Type)member).FullName
                : member.DeclaringType.FullName;
        }

        /// <summary>
        /// Gets the test cases.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public IEnumerable<TestCaseAttribute> GetTestCases(MemberInfo member)
        {
            var cases = member.GetCustomAttributes<TestCaseAttribute>();
            return cases.Any()
                ? cases
                : EmptyTestCase;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public string GetVersion(MemberInfo member)
        {
            var versionAttribute = member.GetCustomAttributes().OfType<VersionAttribute>();

            return versionAttribute == default(IEnumerable<VersionAttribute>) || !versionAttribute.Any()
                ? string.Empty
                : versionAttribute.First().Value;
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        public IEnumerable<string> GetCategories(MemberInfo member)
        {
            return member.GetCustomAttributes<CategoryAttribute>().Select(x => x.Name);
        }

        /// <summary>
        /// Determines whether the specified member has valid attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="member">The member.</param>
        /// <param name="includedCategories">The included categories.</param>
        /// <param name="excludedCategories">The excluded categories.</param>
        /// <param name="suite">The suite.</param>
        /// <returns></returns>
        public bool HasValidAttributes<T>(MemberInfo member, IList<string> includedCategories, IList<string> excludedCategories, string suite) where T : Attribute
        {
            var attributes = member.GetCustomAttributes();

            return member.GetCustomAttribute<T>() != default(T)
                && member.GetCustomAttribute<IgnoreAttribute>() == default(IgnoreAttribute)
                && HasValidCategories(member, includedCategories, excludedCategories)
                && HasValidSuite(member, suite);
        }

        /// <summary>
        /// Determines whether the specified member has a valid suite.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="suite">The suite.</param>
        /// <returns></returns>
        public bool HasValidSuite(MemberInfo member, string suite)
        {
            if (string.IsNullOrWhiteSpace(suite))
            {
                return true;
            }

            var suiteAttr = member.GetCustomAttribute<SuiteAttribute>();
            return suiteAttr != default(SuiteAttribute) && suiteAttr.Name.Equals(suite, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified member has valid categories.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="includedCategories">The included categories.</param>
        /// <param name="excludedCategories">The excluded categories.</param>
        /// <param name="testFixtureDefault">if set to <c>true</c> [test fixture default].</param>
        /// <returns></returns>
        public bool HasValidCategories(MemberInfo member, IList<string> includedCategories, IList<string> excludedCategories, bool testFixtureDefault = true)
        {
            // if no categories are passed, just return true
            if (EnumerableHelper.IsNullOrEmpty(includedCategories) && EnumerableHelper.IsNullOrEmpty(excludedCategories))
            {
                return true;
            }

            var categories = member.GetCustomAttributes<CategoryAttribute>();
            var isTestFixture = member.GetCustomAttribute<TestFixtureAttribute>() != default(TestFixtureAttribute);

            // if this is a TestFixture and it has no categories, return true - could still have valud Tests
            if (isTestFixture && !categories.Any())
            {
                return true;
            }

            // if this has one of the included categories, return true
            if (!EnumerableHelper.IsNullOrEmpty(includedCategories) && categories.Any(c => includedCategories.Any(i => i.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return true;
            }

            // if this has one of the excluded categories, return false
            if (!EnumerableHelper.IsNullOrEmpty(excludedCategories) && categories.Any(c => excludedCategories.Any(e => e.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return false;
            }

            // if we get here and this is a TestFixture, return true - could still have valud Tests
            if (isTestFixture)
            {
                return testFixtureDefault;
            }

            // this is a test with no categories, if the TestFixture has no categories we need to return false,
            // if it has a valid included category we should return true
            if (!categories.Any())
            {
                var fixture = member.DeclaringType;
                var fixtureCategories = fixture.GetCustomAttributes<CategoryAttribute>();
                return fixtureCategories.Any() && HasValidCategories(fixture, includedCategories, excludedCategories, false);
            }

            // this is a test, and included categories were passed, return false
            if (!EnumerableHelper.IsNullOrEmpty(includedCategories))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified member has a valid concurrency.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="concurrencyType">Type of the concurrency.</param>
        /// <param name="defaultConcurreny">The default concurreny.</param>
        /// <returns></returns>
        public bool HasValidConcurrency(MemberInfo member, ConcurrencyType concurrencyType, ConcurrencyType defaultConcurreny = ConcurrencyType.Parallel)
        {
            var concurrency = member.GetCustomAttribute<ConcurrencyAttribute>();

            return concurrency == default(ConcurrencyAttribute)
                ? concurrencyType == defaultConcurreny
                : concurrency.ConcurrencyType == concurrencyType;
        }

        #endregion

        #region Method Calls

        /// <summary>
        /// Gets the expected exception.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public ExpectedExceptionAttribute GetExpectedException(MethodInfo method)
        {
            var exceptionAttribute = method.GetCustomAttributes().OfType<ExpectedExceptionAttribute>();

            return EnumerableHelper.IsNullOrEmpty(exceptionAttribute)
                ? default(ExpectedExceptionAttribute)
                : exceptionAttribute.First();
        }

        /// <summary>
        /// Gets the slack channel details.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public SlackAttribute GetSlackChannel(MethodInfo method)
        {
            var slackAttribute = method.GetCustomAttributes().OfType<SlackAttribute>();

            return EnumerableHelper.IsNullOrEmpty(slackAttribute)
                ? default(SlackAttribute)
                : slackAttribute.First();
        }

        /// <summary>
        /// Determines whether the specified method has parameters.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public bool HasParameters(MethodInfo method)
        {
            var parameters = GetParameters(method);
            return parameters != default(ParameterInfo[]) && parameters.Any();
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public ParameterInfo[] GetParameters(MethodInfo method)
        {
            return method.GetParameters();
        }

        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="activator">The activator.</param>
        /// <param name="parameters">The parameters.</param>
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

        /// <summary>
        /// Invokes the specified methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <param name="activator">The activator.</param>
        /// <param name="parameters">The parameters.</param>
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

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="includedCategories">The included categories.</param>
        /// <param name="excludedCategories">The excluded categories.</param>
        /// <param name="tests">The tests.</param>
        /// <param name="suite">The suite.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the suites.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public IEnumerable<string> GetSuites(Type type)
        {
            return type.GetCustomAttributes<SuiteAttribute>().Select(x => x.Name);
        }

        #endregion

    }
}
