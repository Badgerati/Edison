/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Core.Output
{
    public interface IOutputRepository
    {

        string ContentType { get; }
        string OpenTag { get; }
        string CloseTag { get; }

        string ToString(TestResult result, bool withTrail);

    }
}
