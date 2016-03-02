/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SuiteAttribute : Attribute
    {
        public string Name = string.Empty;

        public SuiteAttribute(string name)
        {
            Name = name;
        }
    }
}
