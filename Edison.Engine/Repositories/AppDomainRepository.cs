/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IAppDomainRepository))]
    public class AppDomainRepository : IAppDomainRepository
    {

        #region Repositories

        private IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        #endregion


        public AppDomain CurrentDomain
        {
            get { return AppDomain.CurrentDomain; }
        }


        public void SetAppConfig(string path)
        {
            if (!FileRepository.Exists(path))
            {
                return;
            }

            CurrentDomain.SetData("APP_CONFIG_FILE", path);
        }

    }
}
