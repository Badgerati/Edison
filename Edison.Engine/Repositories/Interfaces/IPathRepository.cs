/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IPathRepository
    {

        string GetDirectoryName(string path);
        string GetFileName(string path);
        string GetFullPath(string path);
        string GetExtension(string path);
        string Combine(params string[] paths);

    }
}
