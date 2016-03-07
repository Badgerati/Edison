/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class TestCaseAttribute : Attribute
    {
        public readonly object[] Parameters;
        public readonly bool Parallel;

        public TestCaseAttribute(params object[] parameters) : this(false, parameters)
        {
        }

        protected TestCaseAttribute(bool parallel, params object[] parameters)
        {
            Parameters = parameters;
            Parallel = parallel;
        }
    }
}
