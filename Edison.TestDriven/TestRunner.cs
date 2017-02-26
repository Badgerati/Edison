/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using System.Reflection;
using Edison.Framework;
using TestDriven.Framework;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using ETestResult = Edison.Framework.TestResult;
using TDTestResult = TestDriven.Framework.TestResult;

namespace Edison.TestDriven
{
    public class TestRunner : ITestRunner
    {

        #region Repositories

        private IReflectionRepository ReflectionRepository
        {
            get { return DIContainer.Instance.Get<IReflectionRepository>(); }
        }

        #endregion

        #region Fields

        private ITestListener _listener = null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Run all tests within an assembly
        /// </summary>
        /// <param name="testListener"></param>
        /// <param name="assembly"></param>
        /// <returns>The state of running the tests</returns>
        public TestRunState RunAssembly(ITestListener testListener, Assembly assembly)
        {
            var context = SetupContext(assembly);
            return Run(testListener, context);
        }

        /// <summary>
        /// Can be one of two things, either run all tests for a given fixture, or run a test itself
        /// </summary>
        /// <param name="testListener"></param>
        /// <param name="assembly"></param>
        /// <param name="member"></param>
        /// <returns>The state of running the tests</returns>
        public TestRunState RunMember(ITestListener testListener, Assembly assembly, MemberInfo member)
        {
            var context = SetupContext(assembly);

            // test
            if (ReflectionRepository.IsMethod(member))
            {
                context.Tests.Add(ReflectionRepository.GetFullNamespace(member));
            }

            // fixture
            else if (ReflectionRepository.IsType(member))
            {
                context.Fixtures.Add(ReflectionRepository.GetNamespace(member));
            }

            // error
            else
            {
                return TestRunState.Error;
            }
            
            return Run(testListener, context);
        }

        /// <summary>
        /// Run all tests within a given namespace
        /// </summary>
        /// <param name="testListener"></param>
        /// <param name="assembly"></param>
        /// <param name="ns"></param>
        /// <returns>The state of running the tests</returns>
        public TestRunState RunNamespace(ITestListener testListener, Assembly assembly, string ns)
        {
            var context = SetupContext(assembly);
            context.Fixtures.Add(ns);
            return Run(testListener, context);
        }

        #endregion

        #region Private Helpers

        private TestRunState Run(ITestListener testListener, EdisonContext context)
        {
            _listener = testListener;
            var result = context.Run();

            if (result.TotalCount == 0)
            {
                return TestRunState.NoTests;
            }

            if (result.TotalFailedCount > 0)
            {
                return TestRunState.Failure;
            }

            return TestRunState.Success;
        }

        private EdisonContext SetupContext(Assembly assembly)
        {
            var context = EdisonContext.Create();
            context.Assemblies.Add(assembly.Location);
            context.DisableFileOutput = true;
            context.ConsoleOutputType = OutputType.Txt;
            context.OnTestResult += EdisonContext_OnTestResult;

            return context;
        }

        #endregion

        #region Events

        private void EdisonContext_OnTestResult(ETestResult result)
        {
            switch (result.AbsoluteState)
            {
                case TestResultAbsoluteState.Ignored:
                case TestResultAbsoluteState.Inconclusive:
                case TestResultAbsoluteState.Unknown:
                    _listener.TestFinished(new TDTestResult
                    {
                        Name = result.TestName,
                        State = TestState.Ignored,
                        Message = result.ErrorMessage
                    });
                    break;

                case TestResultAbsoluteState.Failure:
                case TestResultAbsoluteState.Error:
                    _listener.TestFinished(new TDTestResult
                    {
                        Name = result.TestName,
                        State = TestState.Failed,
                        Message = result.ErrorMessage,
                        StackTrace = result.StackTrace
                    });
                    break;

                case TestResultAbsoluteState.Success:
                    _listener.TestFinished(new TDTestResult
                    {
                        Name = result.TestName,
                        State = TestState.Passed
                    });
                    break;
            }
        }

        #endregion

    }
}
