﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Utilities.Structures;
using Edison.Framework;
using Edison.Injector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
        private EdisonContext Context = default(EdisonContext);
        private TestResultDictionary ResultQueue = default(TestResultDictionary);
        private Exception GlobalSetupException = default(Exception);
        private Exception FixtureSetupException = default(Exception);
        private Exception ActivatorException = default(Exception);

        #endregion

        #region Constructor

        public TestThread(int threadId, TestResultDictionary resultQueue, IEnumerable<MethodInfo> tests, Type testFixture,
            int testFixtureRepeatIndex, TestCaseAttribute testFixtureCase, object activator, Exception globalSetupEx,
            Exception fixtureSetupEx, Exception activatorEx, EdisonContext context, ConcurrencyType concurrenyType)
        {
            ThreadId = threadId;
            Context = context;
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

            if (repeat.Parallel)
            {
                // parallel repeats
                var tasks = Task.Run(() => Parallel.ForEach(Enumerable.Range(1, repeat.Value), value => RunTestCases(test, cases, value, setup, teardown)));
                Task.WaitAll(tasks);
            }
            else
            {
                // sequential repeats
                for (var r = 1; r <= repeat.Value; r++)
                {
                    if (Interrupted)
                    {
                        return;
                    }

                    RunTestCases(test, cases, r, setup, teardown);
                }
            }
        }

        private void RunTestCases(MethodInfo test, IEnumerable<TestCaseAttribute> cases, int testRepeat, IEnumerable<MethodInfo> setup, IEnumerable<MethodInfo> teardown)
        {
            var sequentialCases = cases.Where(x => !x.Parallel).ToList();
            foreach (var testcase in sequentialCases)
            {
                if (Interrupted)
                {
                    return;
                }

                RunTestCase(test, testcase, testRepeat, setup, teardown);
            }

            if (sequentialCases.Count == cases.Count())
            {
                return;
            }

            var parallelCases = cases.Where(x => x.Parallel).ToList();
            if (parallelCases.Any())
            {
                var tasks = Task.Run(() => Parallel.ForEach(parallelCases, testcase => RunTestCase(test, testcase, testRepeat, setup, teardown)));
                Task.WaitAll(tasks);
            }
        }

        /// <summary>
        /// Runs the test cases for a test.
        /// </summary>
        /// <param name="test">The test.</param>
        /// <param name="testCase">The test case.</param>
        /// <param name="testRepeat">The test repeat value.</param>
        /// <param name="setup">The setup method for the test.</param>
        /// <param name="teardown">The teardown method for the test.</param>
        private void RunTestCase(MethodInfo test, TestCaseAttribute testCase, int testRepeat, IEnumerable<MethodInfo> setup, IEnumerable<MethodInfo> teardown)
        {
            var duration = new Stopwatch();
            var testResult = default(TestResult);

            var setupDone = false;
            var teardownDone = false;
            var testDone = false;

            try
            {
                // create an initial test result
                testResult = new TestResult(
                    TestResultState.Success,
                    Context.CurrentAssembly,
                    test,
                    TestFixtureCase.Parameters,
                    testCase.Parameters,
                    TestFixtureRepeatIndex,
                    testRepeat,
                    string.Empty,
                    string.Empty,
                    TimeSpan.Zero,
                    string.Empty,
                    default(IEnumerable<string>));

                // start the stop watch for test duration
                duration.Restart();

                // if the global setup failed, this case auto-fails
                if (GlobalSetupException != default(Exception))
                {
                    testResult = PopulateTestResultOnException(test, testResult, GlobalSetupException, false, false, setupDone, teardownDone, testDone, duration.Elapsed);
                }

                // if the activation of the method failed, this case auto-fails
                else if (ActivatorException != default(Exception))
                {
                    testResult = PopulateTestResultOnException(test, testResult, ActivatorException, true, true, true, true, true, duration.Elapsed);
                }

                // if the test fixture setup failed, this case auto-fails
                else if (FixtureSetupException != default(Exception))
                {
                    testResult = PopulateTestResultOnException(test, testResult, FixtureSetupException, true, false, setupDone, teardownDone, testDone, duration.Elapsed);
                }

                // otherwise, run the test case
                else
                {
                    // run the test's setup
                    ReflectionRepository.Invoke(setup, Activator);
                    setupDone = true;

                    // run the test case
                    ReflectionRepository.Invoke(test, Activator, testCase.Parameters);
                    testDone = true;

                    // populate the initial test result as success
                    testResult = PopulateTestResult(test, testResult, TestResultState.Success, duration.Elapsed);

                    // run the test's teardown
                    ReflectionRepository.Invoke(teardown, Activator, testResult);
                    teardownDone = true;
                }

                // stop timer for duration
                duration.Stop();
            }
            catch (Exception ex)
            {
                // the test case failed, populate the result as failed
                testResult = PopulateTestResultOnException(test, testResult, ex, true, true, setupDone, teardownDone, testDone, duration.Elapsed);

                // attempt to the the test's teardown if it wasn't the teardown that failed first time
                if (testResult.State != TestResultState.TeardownError && testResult.State != TestResultState.TeardownFailure)
                {
                    try
                    {
                        ReflectionRepository.Invoke(teardown, Activator, testResult);
                    }
                    catch (Exception ex2)
                    {
                        // if the teardown failed, populate the result appropriately
                        testResult = PopulateTestResultOnException(test, testResult, ex2, true, true, true, false, true, duration.Elapsed);
                    }
                }

                // if the timer is still running, stop it
                if (duration.IsRunning)
                {
                    duration.Stop();
                }
            }

            // add the test's result to the result queue for later processing
            ResultQueue.AddOrUpdate(testResult);
        }

        private TestResult PopulateTestResultOnException(MethodInfo testMethod, TestResult result, Exception ex, bool globalSetup, bool fixSetup, bool setup, bool teardown, bool test, TimeSpan duration)
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
                if (hasInner && CheckExpectedException(testMethod, isAssertFail, ex.InnerException))
                {
                    return PopulateTestResult(testMethod, result, TestResultState.Success, duration);
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

            return PopulateTestResult(testMethod, result, state, duration, error, stack);
        }

        /// <summary>
        /// Populates the test result.
        /// </summary>
        /// <param name="testMethod">The test method.</param>
        /// <param name="result">The result to populate.</param>
        /// <param name="state">The actual test result.</param>
        /// <param name="duration">The duration of the test.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <returns>A populated test result.</returns>
        private TestResult PopulateTestResult(MethodInfo testMethod, TestResult result, TestResultState state, TimeSpan duration, string errorMessage = "", string stackTrace = "")
        {
            // populate general values
            result.State = state;
            result.ErrorMessage = errorMessage;
            result.StackTrace = stackTrace;
            result.Duration = duration;

            // if we have a test object, get test attribute values
            if (testMethod != default(MethodInfo))
            {
                result.Authors = ReflectionRepository.GetAuthors(testMethod);
                result.Version = ReflectionRepository.GetVersion(testMethod);

                // attempt to get slack channel details
                var slack = ReflectionRepository.GetSlackChannel(testMethod);
                if (slack != default(SlackAttribute))
                {
                    result.SlackChannel = slack.Channel;
                    result.SlackTestResult = slack.SlackTestResult;
                }
            }

            return result;
        }

        private bool CheckExpectedException(MethodInfo testMethod, bool isAssertFail, Exception innerException)
        {
            var expectedException = ReflectionRepository.GetExpectedException(testMethod);

            if (expectedException == default(ExpectedExceptionAttribute)
                || expectedException.ExpectedException != innerException.GetType())
            {
                return false;
            }

            if (isAssertFail && !expectedException.AllowAssertException)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(expectedException.ExpectedMessage))
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
