﻿/*
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
    [InjectionInterface(typeof(IFileRepository))]
    public class FileRepository : IFileRepository
    {

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        public FileStream Create(string path)
        {
            return File.Create(path);
        }

        public void AppendAllText(string path, string contents)
        {
            File.AppendAllText(path, contents);
        }

    }
}