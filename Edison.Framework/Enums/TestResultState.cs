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

namespace Edison.Framework.Enums
{
    public enum TestResultState
    {
        //general
        Success,
        Failure,
        Error,
        Inconclusive,
        Ignored,

        //setup
        SetupError,
        SetupFailure,

        //teardown
        TeardownError,
        TeardownFailure,

        //global setup
        GlobalSetupError,
        GlobalSetupFailure,

        //global teardown
        GlobalTeardownError,
        GlobalTeardownFailure,

        //test fixture setup
        TestFixtureSetupError,
        TestFixtureSetupFailure,

        //test fixture teardown
        TestFixtureTeardownError,
        TestFixtureTeardownFailure
    }
}
