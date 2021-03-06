﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;

namespace Edison.Framework
{
    public class AssertException : TestResultStateException
    {

        #region Constructor

        public AssertException(string message, TestResultState state = TestResultState.Failure)
            : base(message, state) { }

        public AssertException(string message, Exception inner, TestResultState state = TestResultState.Failure)
            : base(message, inner, state) { }

        #endregion

    }
}
