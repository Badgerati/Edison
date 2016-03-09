/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.IO;
using System.Text;

namespace Edison.Engine.Repositories
{
    [InjectionInterface(typeof(IFileRepository))]
    public class FileRepository : IFileRepository
    {

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string[] ReadAllLines(string path, Encoding encoding = null)
        {
            return File.ReadAllLines(path, encoding == default(Encoding) ? Encoding.UTF8 : encoding);
        }

        public string ReadAllText(string path, Encoding encoding = null)
        {
            return File.ReadAllText(path, encoding == default(Encoding) ? Encoding.UTF8 : encoding);
        }

        public FileStream Create(string path)
        {
            return File.Create(path);
        }

        public void AppendAllText(string path, string contents, Encoding encoding = null)
        {
            File.AppendAllText(path, contents, encoding == default(Encoding) ? Encoding.UTF8 : encoding);
        }

    }
}
