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
        public string Name { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public DateTime CreateDateTime { get; set; }
        public int TestRepeatIndex { get; set; }
        public int TestFixtureRepeatIndex { get; set; }
        public string Version { get; set; }
        public IEnumerable<string> Authors { get; set; }

        public string CreateDateTimeString
        {
            get { return CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        #endregion

        #region Constructor

        public TestResult(
            TestResultState state,
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
            TestRepeatIndex = testRepeatIndex;
            TestFixtureRepeatIndex = testFixtureRepeatIndex;
            Name = GetName(test, GetParameters(fixtureParameters), GetParameters(testParameters));
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
            var _parameters = string.Empty;

            if (parameters == default(object[]) || parameters.Length == 0)
            {
                return _parameters;
            }

            foreach (var parameter in parameters)
            {
                if (parameter == default(object))
                {
                    _parameters += "NULL, ";
                    continue;
                }

                var paramType = parameter.GetType();
                if (paramType == typeof(string))
                {
                    _parameters += ("\"" + parameter + "\", ");
                }
                else
                {
                    _parameters += (parameter + ", ");
                }
            }

            return _parameters.Trim(',', ' ');
        }

        private string GetName(MethodInfo test, string fixtureParameters, string testParameters)
        {
            var name = new StringBuilder();
            name.Append(test.DeclaringType.FullName + "(" + fixtureParameters + ")");

            if (TestFixtureRepeatIndex != -1)
            {
                name.Append("_" + (TestFixtureRepeatIndex + 1));
            }

            name.Append("." + test.Name + "(" + testParameters + ")");

            if (TestRepeatIndex != -1)
            {
                name.Append("_" + (TestRepeatIndex + 1));
            }

            return name.ToString();
        }

        #endregion

    }
}
