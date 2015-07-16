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
    public class TestResultStateException : Exception
    {

        #region Properties

        public virtual TestResultState TestResultState { get; set; }

        #endregion

        #region Constructor

        public TestResultStateException(string message, TestResultState state)
            : base(message)
        {
            TestResultState = state;
        }

        public TestResultStateException(string message, Exception inner, TestResultState state)
            : base(message, inner)
        {
            TestResultState = state;
        }

        #endregion

    }
}
