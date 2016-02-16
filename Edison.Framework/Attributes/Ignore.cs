/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
        public string Reason = string.Empty;

        public IgnoreAttribute(string reason = "")
        {
            Reason = reason;
        }
    }
}
