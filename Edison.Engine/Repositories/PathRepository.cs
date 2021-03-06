﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.IO;
using System.Linq;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IPathRepository))]
    public class PathRepository : IPathRepository
    {

        #region Repositories

        private IDirectoryRepository DirectoryRepository
        {
            get { return DIContainer.Instance.Get<IDirectoryRepository>(); }
        }

        #endregion

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public string GetTempPath()
        {
            return Path.GetTempPath();
        }

        public string GetRandomFileName()
        {
            return Path.GetRandomFileName();
        }

        public string GetRandomTempPath()
        {
            var temp = GetTempPath();
            var path = string.Empty;
            while (DirectoryRepository.Exists((path = Combine(temp, GetRandomFileName())))) { }
            return path;
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths.Select(x => x.Trim('\\', '/')).ToArray());
        }

    }
}
