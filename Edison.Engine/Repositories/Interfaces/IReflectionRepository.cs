/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Edison.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IReflectionRepository
    {

        IEnumerable<string> GetSuites(Type type);

        IEnumerable<MethodInfo> GetMethods<T>(Type type,
            IList<string> includedCategories = default(List<string>),
            IList<string> excludedCategories = default(List<string>),
            IList<string> tests = default(List<string>),
            string suite = null) where T : Attribute;



        void Invoke(IEnumerable<MethodInfo> methods, object activator, params object[] parameters);

        void Invoke(MethodInfo method, object activator, params object[] parameters);

        bool HasParameters(MethodInfo method);

        ParameterInfo[] GetParameters(MethodInfo method);

        ExpectedExceptionAttribute GetExpectedException(MethodInfo method);



        IEnumerable<TestCaseAttribute> GetTestCases(MemberInfo member);

        IEnumerable<string> GetAuthors(MemberInfo member);

        string GetVersion(MemberInfo member);

        string GetNamespace(MemberInfo member);

        string GetFullNamespace(MemberInfo member);

        IEnumerable<string> GetCategories(MemberInfo member);

        int GetRepeatValue(MemberInfo member);

        bool HasValidAttributes<T>(MemberInfo member, IList<string> includedCategories, IList<string> excludedCategories, string suite) where T : Attribute;

        bool HasValidCategories(MemberInfo member, IList<string> includedCategories, IList<string> excludedCategories);

        bool HasValidConcurrency(MemberInfo member, ConcurrencyType concurrencyType, ConcurrencyType defaultConcurreny = ConcurrencyType.Parallel);

        bool HasValidSuite(MemberInfo member, string suite);

    }
}
