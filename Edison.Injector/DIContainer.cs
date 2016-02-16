/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Injector
{
    public class DIContainer : IDisposable
    {

        #region Properties

        public static Lazy<DIContainer> _lazy = new Lazy<DIContainer>(() => new DIContainer());
        public static IDIContainer Instance
        {
            get { return _lazy.Value.Container; }
            set { _lazy.Value.Container = value; }
        }

        public IDIContainer Container = default(IDIContainer);

        #endregion

        #region Constructor

        private DIContainer()
        {
            Container = new NinjectContainer();
        }

        #endregion

        #region Public Helpers

        public void Dispose()
        {
            if (Container != default(IDIContainer))
            {
                Container.Dispose();
            }
        }

        #endregion


    }
}
