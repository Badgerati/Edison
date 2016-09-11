/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.IO;
using System.Security.AccessControl;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IDirectoryRepository))]
    public class DirectoryRepository : IDirectoryRepository
    {

        #region Repositories

        private IPathRepository PathRepository
        {
            get { return DIContainer.Instance.Get<IPathRepository>(); }
        }

        private IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        #endregion

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public DirectoryInfo CreateTempDirectory()
        {
            return CreateDirectory(PathRepository.GetRandomTempPath());
        }

        public void Delete(string path, bool recursive)
        {
            Directory.Delete(path, recursive);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public DirectoryInfo GetParent(string path)
        {
            return Directory.GetParent(path);
        }

        public void Copy(string source, string target, bool recursive)
        {
            CreateDirectory(target);
            Copy(new DirectoryInfo(source), new DirectoryInfo(target), recursive);
        }

        public void Copy(DirectoryInfo source, DirectoryInfo target, bool recursive)
        {
            CreateDirectory(target.FullName);

            foreach (var file in source.GetFiles())
            {
                file.CopyTo(PathRepository.Combine(target.FullName, file.Name), true);
            }

            if (recursive)
            {
                foreach (var subDir in source.GetDirectories())
                {
                    Copy(subDir, target.CreateSubdirectory(subDir.Name), true);
                }
            }
        }

    }
}
