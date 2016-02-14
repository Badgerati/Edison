﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IFileRepository
    {

        bool Exists(string path);
        string[] ReadAllLines(string path);

    }
}
