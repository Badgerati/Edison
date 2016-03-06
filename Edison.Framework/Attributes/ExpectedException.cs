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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ExpectedExceptionAttribute : Attribute
    {
        public readonly Type ExpectedException;

        private string _expectedMessage = string.Empty;
        public string ExpectedMessage
        {
            get { return _expectedMessage; }
            set { _expectedMessage = value; }
        }

        private MatchType _matchType = MatchType.Exact;
        public MatchType MatchType
        {
            get { return _matchType; }
            set { _matchType = value; }
        }

        private bool _inverseMatch = false;
        public bool InverseMatch
        {
            get { return _inverseMatch; }
            set { _inverseMatch = value; }
        }

        public ExpectedExceptionAttribute(Type exception)
        {
            ExpectedException = exception;
        }
    }
}
