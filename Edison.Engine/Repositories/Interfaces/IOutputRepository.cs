/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Edison.Framework;

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IOutputRepository
    {

        OutputType OutputType { get; }
        string ContentType { get; }
        string OpenTag { get; }
        string CloseTag { get; }
        string Extension { get; }

        string ToString(TestResult result, bool withTrail);

    }
}
