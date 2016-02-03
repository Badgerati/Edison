/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Core.Enums
{
    public enum OutputType : int
    {
        Xml = 1,
        Json = 2,
        Txt = 3,
        Csv = 4,
        Dot = 5,
        None = 6
    }
}
