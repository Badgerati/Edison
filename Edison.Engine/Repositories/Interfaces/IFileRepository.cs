﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System.IO;
using System.Text;

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IFileRepository
    {

        bool Exists(string path);
        string[] ReadAllLines(string path, Encoding encoding);
        string ReadAllText(string path, Encoding encoding);
        FileStream Create(string path);
        void AppendAllText(string path, string contents, Encoding encoding);
        void WriteAllLines(string path, string[] lines, Encoding encoding);
        void WriteAllText(string path, string text, Encoding encoding);
        void Copy(string source, string target, bool overwrite);

    }
}
