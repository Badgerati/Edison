/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using System.IO;

namespace Edison.Console.Test.TestRepositories
{
    public class MockFileRepository : IFileRepository
    {

        private bool existsValue = false;
        private string[] readAllLinesValue = new string[0];


        public MockFileRepository(bool exists)
        {
            existsValue = exists;
        }

        public MockFileRepository(bool exists, string[] readAllLines)
        {
            existsValue = exists;
            readAllLinesValue = readAllLines;
        }


        public bool Exists(string path)
        {
            return existsValue;
        }

        public string[] ReadAllLines(string path)
        {
            return readAllLinesValue;
        }

        public FileStream Create(string path)
        {
            return default(FileStream);
        }

        public void AppendAllText(string path, string contents)
        {
            return;
        }

    }
}
