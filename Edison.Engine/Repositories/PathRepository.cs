/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */
 
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.IO;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IPathRepository))]
    public class PathRepository : IPathRepository
    {

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

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

    }
}
