﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Utilities.Structures;
using Edison.Framework;
using Edison.Framework.Enums;
using Edison.Injector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Edison.Engine.Threading
{
    public class TestThread
    {

        #region Repositories

        private IReflectionRepository ReflectionRepository
        {
            get { return DIContainer.Instance.Get<IReflectionRepository>(); }
        }

        #endregion

        #region Properties
        
        private object Activator = default(object);
        private Type TestFixture = default(Type);
        private int TestFixtureRepeatIndex = default(int);
        private TestCaseAttribute TestFixtureCase = default(TestCaseAttribute);
        private IEnumerable<MethodInfo> Tests = default(IEnumerable<MethodInfo>);
        
        private int ThreadId = default(int);
        private bool Interrupted { get; set; }
        private TestResultDictionary ResultQueue = default(TestResultDictionary);
        private Exception GlobalSetupException = default(Exception);
        private Exception FixtureSetupException = default(Exception);
        private Exception ActivatorException = default(Exception);

        #endregion

        #region Constructor

        public TestThread(int threadId, TestResultDictionary resultQueue, IEnumerable<MethodInfo> tests, Type testFixture,
            int testFixtureRepeatIndex, TestCaseAttribute testFixtureCase, object activator, Exception globalSetupEx,
            Exception fixtureSetupEx, Exception activatorEx, ConcurrencyType concurrenyType)
        {
            ThreadId = threadId;
            ResultQueue = resultQueue;
            TestFixture = testFixture;
            TestFixtureRepeatIndex = testFixtureRepeatIndex;
            Activator = activator;
            TestFixtureCase = testFixtureCase;
            GlobalSetupException = globalSetupEx;
            FixtureSetupException = fixtureSetupEx;
            ActivatorException = activatorEx;
            Tests = tests;
        }

        #endregion

        #region Public Methods

        public void RunTests()
        {
            var setup = ReflectionRepository.GetMethods<SetupAttribute>(TestFixture);
            var teardown = ReflectionRepository.GetMethods<TeardownAttribute>(TestFixture);

            foreach (var test in Tests)
            {
                if (Interrupted)
                {
                    return;
                }

                RunTestRepeats(test, setup, teardown);
            }
        }

        public void Interrupt()
        {
            Interrupted = true;
        }

        #endregion

        #region Private Methods
        
        private void RunTestRepeats(MethodInfo test, IEnumerable<MethodInfo> setup, IEnumerable<MethodInfo> teardown)
        {
            var repeat = ReflectionRepository.GetRepeatValue(test);
            var cases = ReflectionRepository.GetTestCases(test);

            for (var r = 0; r < (repeat == -1 ? 1 : repeat); r++)
            {
                if (Interrupted)
                {
                    return;
                }

                RunTestCases(test, cases, (repeat == -1 ? -1 : r), setup, teardown);
            }
        }

        private void RunTestCases(MethodInfo test, IEnumerable<TestCaseAttribute> cases, int testRepeat, IEnumerable<MethodInfo> setup, IEnumerable<MethodInfo> teardown)
        {
            var timeTaken = new Stopwatch();
            var testResult = default(TestResult);

            var setupDone = false;
            var teardownDone = false;
            var testDone = false;

            foreach (var _case in cases)
            {
                if (Interrupted)
                {
                    return;
                }

                try
                {
                    testResult = new TestResult(
                        TestResultState.Success,
                        test,
                        TestFixtureCase.Parameters,
                        _case.Parameters,
                        TestFixtureRepeatIndex,
                        testRepeat,
                        string.Empty,
                        string.Empty,
                        TimeSpan.Zero,
                        string.Empty,
                        default(IEnumerable<string>));

                    setupDone = false;
                    teardownDone = false;
                    testDone = false;
                    timeTaken.Restart();

                    if (GlobalSetupException != default(Exception))
                    {
                        testResult = PopulateTestResultOnException(test, testResult, GlobalSetupException, false, false, setupDone, teardownDone, testDone, timeTaken.Elapsed);
                    }
                    else if (ActivatorException != default(Exception))
                    {
                        testResult = PopulateTestResultOnException(test, testResult, ActivatorException, true, true, true, true, true, timeTaken.Elapsed);
                    }
                    else if (FixtureSetupException != default(Exception))
                    {
                        testResult = PopulateTestResultOnException(test, testResult, FixtureSetupException, true, false, setupDone, teardownDone, testDone, timeTaken.Elapsed);
                    }
                    else
                    {
                        //setup
                        ReflectionRepository.Invoke(setup, Activator);
                        setupDone = true;

                        //test
                        ReflectionRepository.Invoke(test, Activator, _case.Parameters);
                        testDone = true;

                        testResult = PopulateTestResult(test, testResult, TestResultState.Success, timeTaken.Elapsed);

                        //teardown
                        ReflectionRepository.Invoke(teardown, Activator, testResult);
                        teardownDone = true;
                    }

                    timeTaken.Stop();
                }
                catch (Exception ex)
                {
                    testResult = PopulateTestResultOnException(test, testResult, ex, true, true, setupDone, teardownDone, testDone, timeTaken.Elapsed);

                    //teardown
                    if (testResult.State != TestResultState.TeardownError && testResult.State != TestResultState.TeardownFailure)
                    {
                        try
                        {
                            ReflectionRepository.Invoke(teardown, Activator, testResult);
                        }
                        catch (Exception ex2)
                        {
                            testResult = PopulateTestResultOnException(test, testResult, ex2, true, true, true, false, true, timeTaken.Elapsed);
                        }
                    }

                    if (timeTaken.IsRunning)
                    {
                        timeTaken.Stop();
                    }
                }

                ResultQueue.AddOrUpdate(testResult);
            }
        }

        private TestResult PopulateTestResultOnException(MethodInfo testMethod, TestResult result, Exception ex, bool globalSetup, bool fixSetup, bool setup, bool teardown, bool test, TimeSpan time)
        {
            var hasInner = ex.InnerException != default(Exception);
            var innerExceptionType = hasInner ? ex.InnerException.GetType() : default(Type);
            var isAssertFail = innerExceptionType == typeof(AssertException);
            var assertEx = isAssertFail ? (AssertException)ex.InnerException : default(AssertException);
            var error = isAssertFail ? ex.InnerException.Message : (hasInner ? ex.InnerException.Message : ex.Message);
            var stack = isAssertFail ? ex.InnerException.StackTrace : (hasInner ? ex.InnerException.StackTrace : ex.StackTrace);
            var state = TestResultState.Failure;

            if (!globalSetup)
            {
                state = isAssertFail
                    ? (assertEx.TestResultState != TestResultState.Failure
                        ? assertEx.TestResultState
                        : TestResultState.GlobalSetupFailure)
                    : TestResultState.GlobalSetupError;
            }
            else if (!fixSetup)
            {
                state = isAssertFail
                    ? (assertEx.TestResultState != TestResultState.Failure
                        ? assertEx.TestResultState
                        : TestResultState.TestFixtureSetupFailure)
                    : TestResultState.TestFixtureSetupError;
            }
            else if (!setup)
            {
                state = isAssertFail
                    ? (assertEx.TestResultState != TestResultState.Failure
                        ? assertEx.TestResultState
                        : TestResultState.SetupFailure)
                    : TestResultState.SetupError;
            }
            else if (!test)
            {
                if (hasInner && !isAssertFail && CheckExpectedException(testMethod, ex.InnerException))
                {
                    return PopulateTestResult(testMethod, result, TestResultState.Success, time);
                }

                state = isAssertFail
                    ? assertEx.TestResultState
                    : TestResultState.Error;
            }
            else if (!teardown)
            {
                state = isAssertFail
                    ? (assertEx.TestResultState != TestResultState.Failure
                        ? assertEx.TestResultState
                        : TestResultState.TeardownFailure)
                    : TestResultState.TeardownError;
            }
            else
            {
                state = TestResultState.Error;
            }

            return PopulateTestResult(testMethod, result, state, time, error, stack);
        }

        private TestResult PopulateTestResult(MethodInfo testMethod, TestResult result, TestResultState state, TimeSpan time, string errorMessage = "", string stackTrace = "")
        {
            result.State = state;
            result.ErrorMessage = errorMessage;
            result.StackTrace = stackTrace;
            result.TimeTaken = time;

            if (testMethod != default(MethodInfo))
            {
                result.Authors = ReflectionRepository.GetAuthors(testMethod);
                result.Version = ReflectionRepository.GetVersion(testMethod);
            }

            return result;
        }

        private bool CheckExpectedException(MethodInfo testMethod, Exception innerException)
        {
            var expectedException = ReflectionRepository.GetExpectedException(testMethod);

            if (expectedException == default(ExpectedExceptionAttribute)
                || expectedException.ExpectedException != innerException.GetType())
            {
                return false;
            }

            if (string.IsNullOrEmpty(expectedException.ExpectedMessage))
            {
                return true;
            }

            var matches = false;
            var innerMessage = innerException.Message;
            var expectedMessage = expectedException.ExpectedMessage;

            switch (expectedException.MatchType)
            {
                case MatchType.Exact:
                    matches = string.Equals(innerMessage, expectedMessage);
                    break;

                case MatchType.Contains:
                    matches = innerMessage.Contains(expectedMessage);
                    break;

                case MatchType.RegEx:
                    matches = Regex.IsMatch(innerMessage, expectedMessage);
                    break;

                case MatchType.StartsWith:
                    matches = innerMessage.StartsWith(expectedMessage);
                    break;

                case MatchType.EndsWith:
                    matches = innerMessage.EndsWith(expectedMessage);
                    break;
            }

            return expectedException.InverseMatch
                ? !matches
                : matches;
        }

        #endregion

    }
}