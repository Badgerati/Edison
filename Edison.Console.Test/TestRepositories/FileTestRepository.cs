/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;

namespace Edison.Console.Test.TestRepositories
{
    public class FileTestRepository : IFileRepository
    {

        public bool existsValue = false;
        public string[] readAllLinesValue = new string[0];


        public bool Exists(string path)
        {
            return existsValue;
        }

        public string[] ReadAllLines(string path)
        {
            return readAllLinesValue;
        }

    }
}
