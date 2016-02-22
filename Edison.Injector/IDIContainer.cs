/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;

namespace Edison.Injector
{
    public interface IDIContainer : IDisposable
    {

        void ClearCache();
        void Bind(Type binder, Type bindee);
        void Bind<T, U>() where U : T;
        T BindAndCache<T, U>(IDictionary<string, object> parameters) where U : T;
        T BindAndCacheInstance<T>(T instance);
        void Unbind<T>();
        T Get<T>(IDictionary<string, object> parameters = null, bool overwrite = false);

    }
}
