﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
using System;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExpectedExceptionAttribute : Attribute
    {
        public Type ExpectedException = default(Type);
        public string ExpectedMessage = string.Empty;
        public MatchType MatchType = MatchType.Exact;
        public bool InverseMatch = false;

        public ExpectedExceptionAttribute(Type exception, string message = "", MatchType matchType = MatchType.Exact, bool inverseMatch = false)
        {
            ExpectedException = exception;
            ExpectedMessage = message;
            MatchType = matchType;
            InverseMatch = inverseMatch;
        }
    }
}
