﻿/*
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
        public readonly string Name;

        public SuiteAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Suite name cannot be null or empty.");
            }

            Name = name;
        }
    }
}
