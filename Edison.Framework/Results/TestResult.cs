/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Edison.Framework
{
    public class TestResult
    {

        #region Properties

        public TestResultState State { get; set; }
        public string NameSpace { get; set; }
        public string TestName { get; set; }
        public string FullName { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int TestRepeatIndex { get; set; }
        public int TestFixtureRepeatIndex { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public string Assembly { get; set; }

        public string CreateDateTimeString
        {
            get { return CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public string BasicName
        {
            get { return (NameSpace + "." + TestName); }
        }

        [Obsolete("This property will soon be deprecated, please use FullName instead.")]
        public string Name
        {
            get { return FullName; }
            set { FullName = value; }
        }

        #endregion

        #region Constructor

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
            TimeSpan timeTaken,
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
            TimeTaken = timeTaken;
            Version = version;
            Authors = authors;
            CreateDateTime = DateTime.Now;
        }

        #endregion

        #region Private Helpers

        private string GetParameters(object[] parameters)
        {
            var _parameters = new StringBuilder();

            if (parameters == default(object[]) || parameters.Length == 0)
            {
                return string.Empty;
            }

            foreach (var parameter in parameters)
            {
                if (parameter == default(object))
                {
                    _parameters.Append("NULL");
                }
                else
                {
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

            return _parameters.ToString().Trim(',', ' ');
        }

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
