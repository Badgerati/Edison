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
using Edison.Engine.Utilities.Helpers;

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

        private IList<TestThread> ParallelThreads = default(IList<TestThread>);
        private Task ParallelTask = default(Task);
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
            
            if (ParallelThreads != default(IList<TestThread>))
            {
                foreach (var thread in ParallelThreads)
                {
                    thread.Interrupt();
                }

                Task.WaitAll(ParallelTask);
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
            
            for (var r = 1; r <= repeat.Value; r++)
            {
                if (Interrupted)
                {
                    return;
                }

                //cases
                RunTestFixtureCases(testFixture, cases, r, fixtureSetup, fixtureTeardown);
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
            var tests = ReflectionRepository.GetMethods<TestAttribute>(testFixture, Context.IncludedCategories, Context.ExcludedCategories, Context.Tests).ToList();

            if (!tests.Any())
            {
                return;
            }

            #region Parallel

            if (NumberOfTestThreads > 1 && tests.Count() != 1)
            {
                tests = tests.Where(t => ReflectionRepository.HasValidConcurrency(t, ConcurrencyType.Parallel, ConcurrencyType)).OrderBy(t => t.Name).ToList();
            }

            var testsCount = tests.Count();
            if (testsCount < NumberOfTestThreads)
            {
                NumberOfTestThreads = testsCount;
            }

            ParallelThreads = new List<TestThread>(NumberOfTestThreads);
            var segment = testsCount == 0 ? 0 : (int)Math.Round((double)testsCount / (double)NumberOfTestThreads, MidpointRounding.ToEven);

            var threadCount = 1;
            for (threadCount = 1; threadCount <= NumberOfTestThreads; threadCount++)
            {
                var testsSegment = threadCount == NumberOfTestThreads
                    ? tests.Skip((threadCount - 1) * segment)
                    : tests.Skip((threadCount - 1) * segment).Take(segment);

                var thread = new TestThread(threadCount, ResultQueue, testsSegment, testFixture, testFixtureRepeat, testFixtureCase, activator,
                    GlobalSetupException, FixtureSetupException, ActivatorException, ConcurrencyType.Parallel);
                ParallelThreads.Add(thread);
            }
            
            ParallelTask = Task.Run(() => Parallel.ForEach(ParallelThreads, thread => thread.RunTests()));
            Task.WaitAll(ParallelTask);

            #endregion

            #region Singular

            var singularTests = tests.Where(t => ReflectionRepository.HasValidConcurrency(t, ConcurrencyType.Serial, ConcurrencyType)).OrderBy(t => t.Name).ToList();

            if (!EnumerableHelper.IsNullOrEmpty(singularTests))
            {
                SingularThread = new TestThread(threadCount + 1, ResultQueue, singularTests, testFixture, testFixtureRepeat, testFixtureCase, activator,
                    GlobalSetupException, FixtureSetupException, ActivatorException, ConcurrencyType.Serial);
                SingularTask = Task.Factory.StartNew(() => SingularThread.RunTests());
                Task.WaitAll(SingularTask);
            }

            #endregion
        }

        #endregion

    }
}
