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
        private string _reason = string.Empty;
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        public IgnoreAttribute()
        {
        }
    }
}
