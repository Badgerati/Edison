/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Edison.Framework
{
    [Serializable]
    public class TestResult
    {

        #region Properties

        public TestResultAbsoluteState AbsoluteState { get; private set; }

        private TestResultState _state = default(TestResultState);
        public TestResultState State
        {
            get { return _state; }
            set
            {
                _state = value;
                AbsoluteState = TestResultGroup.GetAbsoluteState(_state);
            }
        }

        public string NameSpace { get; set; }
        public string TestName { get; set; }
        public string FullName { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int TestRepeatIndex { get; set; }
        public int TestFixtureRepeatIndex { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public string Assembly { get; set; }
        public string SlackChannel { get; set; }
        public SlackTestResultType SlackTestResult { get; set; }

        #endregion

        #region Extension Properties

        public string CreateDateTimeString
        {
            get { return CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public string BasicName
        {
            get { return (NameSpace + "." + TestName); }
        }

        public bool IsError
        {
            get { return TestResultGroup.Errors.Contains(State); }
        }

        public bool IsFailure
        {
            get { return TestResultGroup.Failures.Contains(State); }
        }

        public bool IsSlackable
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SlackChannel))
                {
                    return false;
                }

                if (SlackTestResult == SlackTestResultType.Any)
                {
                    return true;
                }

                return ((SlackTestResult == SlackTestResultType.Failure && (AbsoluteState == TestResultAbsoluteState.Failure || AbsoluteState == TestResultAbsoluteState.Error))
                    || (SlackTestResult == SlackTestResultType.Success && AbsoluteState == TestResultAbsoluteState.Success));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResult"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="test">The test.</param>
        /// <param name="fixtureParameters">The fixture parameters.</param>
        /// <param name="testParameters">The test parameters.</param>
        /// <param name="testFixtureRepeatIndex">Index of the test fixture repeat.</param>
        /// <param name="testRepeatIndex">Index of the test repeat.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="stackTrace">The stack trace.</param>
        /// <param name="duration">The time taken.</param>
        /// <param name="version">The version.</param>
        /// <param name="authors">The authors.</param>
        /// <param name="slackChannel">The slack channel.</param>
        public TestResult(
            TestResultState state,
            string assembly,
            MethodInfo test,
            object[] fixtureParameters,
            object[] testParameters,
            int testFixtureRepeatIndex,
            int testRepeatIndex,
            string errorMessage,
            string stackTrace,
            TimeSpan duration,
            string version,
            IEnumerable<string> authors)
        {
            State = state;
            Assembly = assembly;
            TestRepeatIndex = testRepeatIndex;
            TestFixtureRepeatIndex = testFixtureRepeatIndex;
            NameSpace = test.DeclaringType.FullName;
            TestName = test.Name;
            FullName = GetName(GetParameters(fixtureParameters), GetParameters(testParameters));
            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
            Duration = duration;
            Version = version;
            Authors = authors;
            CreateDateTime = DateTime.Now;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Constructs a string of the parameters for the test.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A stringified version of the parameters.</returns>
        private string GetParameters(object[] parameters)
        {
            var _parameters = new StringBuilder();

            // If there are no parameters, just return empty
            if (parameters == default(object[]) || parameters.Length == 0)
            {
                return string.Empty;
            }

            foreach (var parameter in parameters)
            {
                // If parameter is null, then use string "NULL"
                if (parameter == default(object))
                {
                    _parameters.Append("NULL");
                }
                else
                {
                    // Append string/char quotes, or just use the value
                    var paramType = parameter.GetType();
                    if (paramType == typeof(string))
                    {
                        _parameters.Append("\"" + parameter + "\"");
                    }
                    else if (paramType == typeof(char))
                    {
                        _parameters.Append("'" + parameter + "'");
                    }
                    else
                    {
                        _parameters.Append(parameter);
                    }
                }

                _parameters.Append(", ");
            }

            // return stringified parameters
            return _parameters.ToString().Trim(',', ' ');
        }

        /// <summary>
        /// Gets the fully quantified name of the test including:
        /// Namespace, Class name and parameters, Test name and parameters.
        /// </summary>
        /// <param name="fixtureParameters">The fixture's parameters.</param>
        /// <param name="testParameters">The test's parameters.</param>
        /// <returns>The fully quantified test name.</returns>
        private string GetName(string fixtureParameters, string testParameters)
        {
            var name = new StringBuilder();
            name.Append(NameSpace + "(" + fixtureParameters + ")");

            if (TestFixtureRepeatIndex > 1)
            {
                name.Append("_" + TestFixtureRepeatIndex);
            }

            name.Append("." + TestName + "(" + testParameters + ")");

            if (TestRepeatIndex > 1)
            {
                name.Append("_" + TestRepeatIndex);
            }

            return name.ToString();
        }

        #endregion

    }

}
