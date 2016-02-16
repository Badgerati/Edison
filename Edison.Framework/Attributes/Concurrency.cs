/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
using System;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConcurrencyAttribute : Attribute
    {
        public ConcurrencyType ConcurrencyType = ConcurrencyType.Parallel;

        public ConcurrencyAttribute(ConcurrencyType concurrentType = ConcurrencyType.Parallel)
        {
            ConcurrencyType = concurrentType;
        }
    }
}
