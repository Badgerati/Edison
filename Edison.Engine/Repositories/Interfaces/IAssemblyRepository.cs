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

        IEnumerable<Type> GetTypes<T>(Assembly assembly,
            IList<string> includedCategories = default(IList<string>),
            IList<string> excludedCategories = default(IList<string>));

        IOrderedEnumerable<Type> GetTestFixtures(Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests);

        IEnumerable<MethodInfo> GetAllTests(Assembly assembly);

        IEnumerable<MethodInfo> GetTests(Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests);

        int GetTestCount(Assembly assembly,
            IList<string> includedCategories,
            IList<string> excludedCategories,
            IList<string> fixtures,
            IList<string> tests);

    }
}
