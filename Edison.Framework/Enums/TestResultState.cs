/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

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

    public static class TestResultGroup
    {

        private static TestResultState[] _errors;
        public static TestResultState[] Errors
        {
            get
            {
                if (_errors == default(TestResultState[]))
                {
                    _errors = new[] { TestResultState.Error,
                        TestResultState.GlobalSetupError,
                        TestResultState.GlobalTeardownError,
                        TestResultState.SetupError,
                        TestResultState.TeardownError,
                        TestResultState.TestFixtureSetupError,
                        TestResultState.TestFixtureTeardownError };
                }

                return _errors;
            }
        }

        private static TestResultState[] _failures;
        public static TestResultState[] Failures
        {
            get
            {
                if (_failures == default(TestResultState[]))
                {
                    _failures = new[] { TestResultState.Failure,
                        TestResultState.GlobalSetupFailure,
                        TestResultState.GlobalTeardownFailure,
                        TestResultState.SetupFailure,
                        TestResultState.TeardownFailure,
                        TestResultState.TestFixtureSetupFailure,
                        TestResultState.TestFixtureTeardownFailure };
                }

                return _failures;
            }
        }

    }
}
