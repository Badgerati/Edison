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

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ConcurrentAttribute : Attribute
    {
        public ConcurrencyType ConcurrentType = ConcurrencyType.Parallel;

        public ConcurrentAttribute(ConcurrencyType concurrentType = ConcurrencyType.Parallel)
        {
            ConcurrentType = concurrentType;
        }
    }
}
