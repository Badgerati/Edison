/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Utilities.Extensions;
using Edison.Framework;
using Edison.Engine.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Edison.Engine.Contexts;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;
using Edison.Engine.Utilities.Structures;
using Edison.Framework.Enums;
using System.Text.RegularExpressions;

namespace Edison.Engine.Threading
{
    public class EdisonTestThread
    {

        #region Properties

        private Thread _thread = default(Thread);
        private IList<Type> _testFixtures = default(IList<Type>);

        public bool IsFinished
        {
            get
            {
                return _thread.ThreadState != ThreadState.Running
                    && _thread.ThreadState != ThreadState.WaitSleepJoin;
            }
        }

        public bool Interrupt { get; set; }

        private int ThreadId = default(int);
        private EdisonContext Context = default(EdisonContext);
        private TestResultDictionary ResultQueue = default(TestResultDictionary);
        private Exception GlobalSetupException = default(Exception);
        private Exception FixtureSetupException = default(Exception);
        private Exception ActivatorException = default(Exception);

        #endregion

        #region Constructor

        public EdisonTestThread(int threadId, EdisonContext context, TestResultDictionary resultQueue, IList<Type> fixtures, Exception globalSetupEx)
        {
            ThreadId = threadId;
            Context = context;
            ResultQueue = resultQueue;
            GlobalSetupException = globalSetupEx;
            _testFixtures = fixtures;
            _thread = new Thread(RunTestFixtures);
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            Interrupt = false;
            _thread.Start();
        }

        #endregion

        #region Private Methods

        private void RunTestFixtures()
        {
            foreach (var testFixture in _testFixtures)
            {
                if (Interrupt)
                {
                    return;
                }

                var setup = testFixture.GetMethods<TestFixtureSetupAttribute>().ToList();
                var teardown = testFixture.GetMethods<TestFixtureTeardownAttribute>().ToList();
                
                //repeats
                RunTestFixtureRepeats(testFixture, setup, teardown);
            }
        }

        private void RunTestFixtureRepeats(Type testFixture, List<MethodInfo> fixtureSetup, List<MethodInfo> fixtureTeardown)
        {
            var repeat = testFixture.GetRepeatValue();
            var cases = testFixture.GetTestCases();
            
            for (var r = 0; r < (repeat == -1 ? 1 : repeat); r++)
            {
                if (Interrupt)
                {
                    return;
                }

                //cases
                RunTestFixtureCases(testFixture, cases, (repeat == -1 ? -1 : r), fixtureSetup, fixtureTeardown);
            }
        }

        private void RunTestFixtureCases(Type testFixture, IList<TestCaseAttribute> cases, int testFixtureRepeat, List<MethodInfo> fixtureSetup, List<MethodInfo> fixtureTeardown)
        {
            var activator = default(object);
            var setupDone = false;
            var testDone = false;

            foreach (var _case in cases)
            {
                if (Interrupt)
                {
                    return;
                }

                try
                {
                    activator = Activator.CreateInstance(testFixture, _case.Parameters);
                }
                catch (Exception ex)
                {
                    ActivatorException = ex;
                }

                try
                {
                    setupDone = false;
                    testDone = false;

                    try
                    {
                        if (GlobalSetupException == default(Exception))
                        {
                            //fixture setup
                            ReflectionHelper.Invoke(fixtureSetup, activator);
                        }

                        setupDone = true;

                        //tests
                        RunTests(testFixture, testFixtureRepeat, _case, activator);
                        testDone = true;
                    }
                    catch (Exception ex)
                    {
                        if (!setupDone)
                        {
                            //tests
                            FixtureSetupException = ex;
                            RunTests(testFixture, testFixtureRepeat, _case, activator);
                            testDone = true;
                        }
                    }

                    //fixture teardown
                    ReflectionHelper.Invoke(fixtureTeardown, activator);
                }
                catch (Exception ex)
                {
                    if (!setupDone || !testDone)
                    {
                        try
                        {
                            //fixture teardown
                            ReflectionHelper.Invoke(fixtureTeardown, activator);
                        }
                        catch { }
                    }
                    else
                    {
                        Logger.Instance.WriteInnerException(ex, true);
                    }
                }
                finally
                {
                    ActivatorException = default(Exception);
                    FixtureSetupException = default(Exception);
                }
            }
        }

        private void RunTests(Type testFixture, int testFixtureRepeat, TestCaseAttribute testFixtureCase, object activator)
        {
            var tests = testFixture.GetMethods<TestAttribute>(Context.IncludedCategories, Context.ExcludedCategories, Context.Tests).ToList();

            if (!tests.Any())
            {
                return;
            }

            var setup = testFixture.GetMethods<SetupAttribute>().ToList();
            var teardown = testFixture.GetMethods<TeardownAttribute>().ToList();

            foreach (var test in tests)
            {
                if (Interrupt)
                {
                    return;
                }

                RunTestRepeats(test, testFixtureRepeat, testFixtureCase, setup, teardown, activator);
            }
        }

        private void RunTestRepeats(MethodInfo test, int testFixtureRepeat, TestCaseAttribute testFixtureCase, List<MethodInfo> setup, List<MethodInfo> teardown, object activator)
        {
            var repeat = test.GetRepeatValue();
            var cases = test.GetTestCases();
            
            for (var r = 0; r < (repeat == -1 ? 1 : repeat); r++)
            {
                if (Interrupt)
                {
                    return;
                }

                RunTestCases(test, testFixtureCase, cases, testFixtureRepeat, (repeat == -1 ? -1 : r), setup, teardown, activator);
            }
        }

        private void RunTestCases(MethodInfo test, TestCaseAttribute testFixtureCase, IList<TestCaseAttribute> cases, int testFixtureRepeat,
            int testRepeat, List<MethodInfo> setup, List<MethodInfo> teardown, object activator)
        {
            var timeTaken = new Stopwatch();
            var testResult = default(TestResult);

            var setupDone = false;
            var teardownDone = false;
            var testDone = false;

            foreach (var _case in cases)
            {
                if (Interrupt)
                {
                    return;
                }

                try
                {
                    testResult = new TestResult(
                        TestResultState.Success,
                        test,
                        testFixtureCase.Parameters,
                        _case.Parameters,
                        testFixtureRepeat,
                        testRepeat,
                        string.Empty,
                        string.Empty,
                        TimeSpan.Zero,
                        string.Empty,
                        default(IList<string>));

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
                        ReflectionHelper.Invoke(setup, activator);
                        setupDone = true;

                        //test
                        ReflectionHelper.Invoke(test, activator, false, _case.Parameters);
                        testDone = true;

                        testResult = PopulateTestResult(test, testResult, TestResultState.Success, timeTaken.Elapsed);

                        //teardown
                        ReflectionHelper.Invoke(teardown, activator, true, testResult);
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
                            ReflectionHelper.Invoke(teardown, activator, true, testResult);
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
            var error = isAssertFail ? ex.InnerException.Message : (hasInner ? ex.InnerException.Message: ex.Message);
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
                result.Authors = testMethod.GetAuthors();
                result.Version = testMethod.GetVersion();
            }

            return result;
        }

        private bool CheckExpectedException(MethodInfo testMethod, Exception innerException)
        {
            var expectedException = testMethod.GetExpectedException();

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
