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

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IAssemblyRepository
    {

        Assembly LoadFrom(string assemblyFile);
        Assembly LoadFile(string path);
        Assembly GetEntryAssembly();
        string GetAssemblyVersion();

        IEnumerable<Type> GetTypes<T>(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            string suite) where T : Attribute;

        IEnumerable<Type> GetTestFixtures(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests,
            string suite);

        Tuple<IEnumerable<MethodInfo>, IEnumerable<Type>> GetTests(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests,
            string suite);

        IEnumerable<string> GetSuites(
            Assembly assembly,
            IEnumerable<Type> fixtures = default(IEnumerable<Type>));

        IEnumerable<string> GetCategories(
            Assembly assembly,
            IEnumerable<MethodInfo> tests = default(IEnumerable<MethodInfo>),
            IEnumerable<Type> fixtures = default(IEnumerable<Type>));

        int GetTestCount(
            Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests,
            string suite);

    }
}
