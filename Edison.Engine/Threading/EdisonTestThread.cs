﻿/*
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
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Edison.Engine.Contexts;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;
using Edison.Engine.Utilities.Structures;

namespace Edison.Engine.Threading
{
    public class EdisonTestThread
    {

        #region Properties

        private Thread _thread = default(Thread);
        private IList<Type> _testFixtures = default(IList<Type>);

        public bool IsFinished
        {
            get { return _thread.ThreadState != ThreadState.Running; }
        }
    
        public int ThreadId { get; private set; }
        public bool Interrupt { get; set; }
        private EdisonContext Context = default(EdisonContext);
        private TestResultDictionary ResultQueue = default(TestResultDictionary);
        private Exception GlobalSetupException = default(Exception);

        #endregion

        #region Constructor

        public EdisonTestThread(int threadId, EdisonContext context, TestResultDictionary resultQueue, IList<Type> fixtures, Exception globalSetupEx)
        {
            ThreadId = threadId;
            Context = context;
            ResultQueue = resultQueue;
            GlobalSetupException = globalSetupEx;
            _testFixtures = fixtures;
            _thread = new Thread(Run);
        }

        #endregion

        #region Public Methods

        public void Start()
        {
            Logger.WriteMessage(string.Format("Starting up EdisonTestThread {0}", ThreadId));

            Interrupt = false;
            _thread.Start();
        }

        #endregion

        #region Private Methods

        private void Run()
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
            var activator = Activator.CreateInstance(testFixture, null);
            var repeat = testFixture.GetCustomAttributes().OfType<RepeatAttribute>().SingleOrDefault();
            var hasRepeat = repeat != default(RepeatAttribute);

            var setupDone = false;
            var testDone = false;

            for (var r = 0; r < (!hasRepeat ? 1 : repeat.Value); r++)
            {
                if (Interrupt)
                {
                    return;
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
                        RunTests(testFixture, (hasRepeat ? r : -1), default(Exception), activator);
                        testDone = true;
                    }
                    catch (Exception ex)
                    {
                        if (!setupDone)
                        {
                            //tests
                            RunTests(testFixture, (hasRepeat ? r : -1), ex, activator);
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
                        Logger.WriteInnerException(ex, true);
                    }
                }
            }
        }

        private void RunTests(Type testFixture, int testFixtureRepeat, Exception fixSetupEx, object activator)
        {
            var tests = testFixture.GetMethods<TestAttribute>(Context.IncludedCategories, Context.ExcludedCategories).ToList();
            var setup = testFixture.GetMethods<SetupAttribute>().ToList();
            var teardown = testFixture.GetMethods<TeardownAttribute>().ToList();

            foreach (var test in tests)
            {
                if (Interrupt)
                {
                    return;
                }

                RunTestRepeats(test, testFixtureRepeat, setup, teardown, fixSetupEx, activator);
            }
        }

        private void RunTestRepeats(MethodInfo test, int testFixtureRepeat, List<MethodInfo> setup, List<MethodInfo> teardown, Exception fixSetupEx, object activator)
        {
            var repeat = test.GetCustomAttributes().OfType<RepeatAttribute>().SingleOrDefault();
            var hasRepeat = repeat != default(RepeatAttribute);
            var cases = test.GetCustomAttributes().OfType<CaseAttribute>().ToList();

            if (cases.Count == 0)
            {
                cases.Add(new CaseAttribute());
            }

            for (var r = 0; r < (!hasRepeat ? 1 : repeat.Value); r++)
            {
                if (Interrupt)
                {
                    return;
                }

                RunTestCases(test, cases, testFixtureRepeat, (hasRepeat ? r : -1), setup, teardown, fixSetupEx, activator);
            }
        }

        private void RunTestCases(MethodInfo test, List<CaseAttribute> cases, int testFixtureRepeat, int testRepeat, List<MethodInfo> setup, List<MethodInfo> teardown, Exception fixSetupEx, object activator)
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
                    testResult = new TestResult(TestResultState.Success, test, _case.Parameters, testFixtureRepeat, testRepeat, string.Empty, string.Empty, TimeSpan.Zero);
                    setupDone = false;
                    teardownDone = false;
                    testDone = false;
                    timeTaken.Restart();

                    if (GlobalSetupException != default(Exception))
                    {
                        testResult = PopulateTestResultOnException(testResult, GlobalSetupException, false, false, setupDone, teardownDone, testDone, timeTaken.Elapsed);
                    }
                    else if (fixSetupEx != default(Exception))
                    {
                        testResult = PopulateTestResultOnException(testResult, fixSetupEx, true, false, setupDone, teardownDone, testDone, timeTaken.Elapsed);
                    }
                    else
                    {
                        //setup
                        ReflectionHelper.Invoke(setup, activator);
                        setupDone = true;

                        //test
                        ReflectionHelper.Invoke(test, activator, false, _case.Parameters);
                        testDone = true;

                        testResult = PopulateTestResult(testResult, TestResultState.Success, timeTaken.Elapsed);

                        //teardown
                        ReflectionHelper.Invoke(teardown, activator, true, testResult);
                        teardownDone = true;
                    }

                    timeTaken.Stop();
                }
                catch (Exception ex)
                {
                    testResult = PopulateTestResultOnException(testResult, ex, true, true, setupDone, teardownDone, testDone, timeTaken.Elapsed);

                    //teardown
                    if (testResult.State != TestResultState.TeardownError && testResult.State != TestResultState.TeardownFailure)
                    {
                        try
                        {
                            ReflectionHelper.Invoke(teardown, activator, true, testResult);
                        }
                        catch (Exception ex2)
                        {
                            testResult = PopulateTestResultOnException(testResult, ex2, true, true, true, false, true, timeTaken.Elapsed);
                        }
                    }

                    if (timeTaken.IsRunning)
                    {
                        timeTaken.Stop();
                    }
                }

                ResultQueue.AddOrUpdate(testResult);

                //write result to file
                Logger.WriteMessage(string.Format("{0}- - - - - - - - - - - - - - - - - - - - - - - -{0}", Environment.NewLine));
            }
        }

        private TestResult PopulateTestResultOnException(TestResult result, Exception ex, bool globalSetup, bool fixSetup, bool setup, bool teardown, bool test, TimeSpan time)
        {
            var hasInner = ex.InnerException != default(Exception);
            var isAssertFail = hasInner && ex.InnerException.GetType() == typeof(AssertException);
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

            return PopulateTestResult(result, state, time, error, stack); ;
        }

        private TestResult PopulateTestResult(TestResult result, TestResultState state, TimeSpan time, string errorMessage = "", string stackTrace = "")
        {
            result.State = state;
            result.ErrorMessage = errorMessage;
            result.StackTrace = stackTrace;
            result.TimeTaken = time;
            return result;
        }

        #endregion

    }
}