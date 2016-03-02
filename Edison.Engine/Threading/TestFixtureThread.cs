/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Edison.Engine.Contexts;
using Edison.Engine.Utilities.Structures;
using Edison.Framework.Enums;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.Threading.Tasks;

namespace Edison.Engine.Threading
{
    public class TestFixtureThread
    {

        #region Repositories

        private IReflectionRepository ReflectionRepository
        {
            get { return DIContainer.Instance.Get<IReflectionRepository>(); }
        }

        #endregion

        #region Properties
        
        private IEnumerable<Type> TestFixtures = default(IEnumerable<Type>);
                
        private int ThreadId = default(int);
        private bool Interrupted = false;
        private EdisonContext Context = default(EdisonContext);
        private TestResultDictionary ResultQueue = default(TestResultDictionary);
        private Exception GlobalSetupException = default(Exception);
        private Exception FixtureSetupException = default(Exception);
        private Exception ActivatorException = default(Exception);
        private int NumberOfTestThreads = default(int);
        private ConcurrencyType ConcurrencyType = ConcurrencyType.Parallel;

        private IList<TestThread> Threads = default(IList<TestThread>);
        private IList<Task> Tasks = default(IList<Task>);
        private TestThread SingularThread = default(TestThread);
        private Task SingularTask = default(Task);

        #endregion

        #region Constructor

        public TestFixtureThread(int threadId, EdisonContext context, TestResultDictionary resultQueue, IEnumerable<Type> fixtures,
            Exception globalSetupEx, ConcurrencyType concurrenyType, int numberOfTestThreads)
        {
            ThreadId = threadId;
            Context = context;
            ResultQueue = resultQueue;
            GlobalSetupException = globalSetupEx;
            TestFixtures = fixtures;
            NumberOfTestThreads = numberOfTestThreads;
        }

        #endregion

        #region Public Methods
        
        public void Interrupt()
        {
            Interrupted = true;
            
            if (Threads != default(IList<TestThread>))
            {
                foreach (var thread in Threads)
                {
                    thread.Interrupt();
                }

                Task.WaitAll(Tasks.ToArray());
            }

            if (SingularThread != default(TestThread))
            {
                SingularThread.Interrupt();
                Task.WaitAll(SingularTask);
            }
        }

        #endregion

        #region Private Methods

        public void RunTestFixtures()
        {
            foreach (var testFixture in TestFixtures)
            {
                if (Interrupted)
                {
                    return;
                }

                var setup = ReflectionRepository.GetMethods<TestFixtureSetupAttribute>(testFixture);
                var teardown = ReflectionRepository.GetMethods<TestFixtureTeardownAttribute>(testFixture);
                
                //repeats
                RunTestFixtureRepeats(testFixture, setup, teardown);
            }
        }

        private void RunTestFixtureRepeats(Type testFixture, IEnumerable<MethodInfo> fixtureSetup, IEnumerable<MethodInfo> fixtureTeardown)
        {
            var repeat = ReflectionRepository.GetRepeatValue(testFixture);
            var cases = ReflectionRepository.GetTestCases(testFixture);
            
            for (var r = 0; r < (repeat == -1 ? 1 : repeat); r++)
            {
                if (Interrupted)
                {
                    return;
                }

                //cases
                RunTestFixtureCases(testFixture, cases, (repeat == -1 ? -1 : r), fixtureSetup, fixtureTeardown);
            }
        }

        private void RunTestFixtureCases(Type testFixture, IEnumerable<TestCaseAttribute> cases, int testFixtureRepeat, IEnumerable<MethodInfo> fixtureSetup, IEnumerable<MethodInfo> fixtureTeardown)
        {
            var activator = default(object);
            var setupDone = false;
            var testDone = false;

            foreach (var _case in cases)
            {
                if (Interrupted)
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
                            ReflectionRepository.Invoke(fixtureSetup, activator);
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
                    ReflectionRepository.Invoke(fixtureTeardown, activator);
                }
                catch (Exception ex)
                {
                    if (!setupDone || !testDone)
                    {
                        try
                        {
                            //fixture teardown
                            ReflectionRepository.Invoke(fixtureTeardown, activator);
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
            var tests = ReflectionRepository.GetMethods<TestAttribute>(testFixture, Context.IncludedCategories, Context.ExcludedCategories, Context.Tests);

            if (!tests.Any())
            {
                return;
            }

            var singularTests = default(IOrderedEnumerable<MethodInfo>);
            if (NumberOfTestThreads > 1 && tests.Count() != 1)
            {
                singularTests = tests.Where(t => ReflectionRepository.HasValidConcurrency(t, ConcurrencyType.Serial, ConcurrencyType)).OrderBy(t => t.Name);
                tests = tests.Where(t => ReflectionRepository.HasValidConcurrency(t, ConcurrencyType.Parallel, ConcurrencyType)).OrderBy(t => t.Name);
            }

            var _tests = tests.Count();
            if (_tests < NumberOfTestThreads)
            {
                NumberOfTestThreads = _tests;
            }

            Threads = new List<TestThread>(NumberOfTestThreads);
            var segment = _tests == 0 ? 0 : (double)_tests / (double)NumberOfTestThreads;

            // setup all the threads that are to be run in parallel
            var i = 1;
            for (i = 1; i <= NumberOfTestThreads; i++)
            {
                var testsSegment = i == NumberOfTestThreads
                    ? tests.Skip((int)((i - 1) * segment))
                    : tests.Skip((int)((i - 1) * segment)).Take((int)(segment));

                var thread = new TestThread(i, ResultQueue, testsSegment, testFixture, testFixtureRepeat, testFixtureCase, activator,
                    GlobalSetupException, FixtureSetupException, ActivatorException, ConcurrencyType.Parallel);
                Threads.Add(thread);
            }

            // setup - if needed - the singular thread
            SingularThread = singularTests != default(IOrderedEnumerable<MethodInfo>)
                ? new TestThread(i + 1, ResultQueue, singularTests, testFixture, testFixtureRepeat, testFixtureCase, activator,
                    GlobalSetupException, FixtureSetupException, ActivatorException, ConcurrencyType.Serial)
                : default(TestThread);

            RunThreads();
        }

        private void RunThreads()
        {
            Tasks = new List<Task>(Threads.Count);
            foreach (var thread in Threads)
            {
                Tasks.Add(Task.Factory.StartNew(() => thread.RunTests()));
            }

            Task.WaitAll(Tasks.ToArray());

            // once finished, we need to run the possible singular tests
            if (SingularThread != default(TestThread))
            {
                SingularTask = Task.Factory.StartNew(() => SingularThread.RunTests());
                Task.WaitAll(SingularTask);
            }
        }

        #endregion

    }
}
