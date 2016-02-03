/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
        public IList<string> Authors { get; set; }

        public string CreateDateTimeString
        {
            get { return CreateDateTime.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        #endregion

        #region Constructor

        public TestResult(
            TestResultState state,
            MethodInfo method,
            object[] parameters,
            int testFixtureRepeatIndex,
            int testRepeatIndex,
            string errorMessage,
            string stackTrace,
            TimeSpan timeTaken,
            string version,
            IList<string> authors)
        {
            State = state;
            TestRepeatIndex = testRepeatIndex;
            TestFixtureRepeatIndex = testFixtureRepeatIndex;

            var _parameters = string.Empty;
            if (parameters != default(object[]) && parameters.Length > 0)
            {
                foreach (var parameter in parameters)
                {
                    _parameters += (parameter + ", ");
                }

                _parameters = _parameters.Trim(',', ' ');
            }

            Name = string.Format("{0}.{1}({2}){3}",
                method.DeclaringType.FullName,
                method.Name,
                _parameters,
                TestRepeatIndex == -1 && TestFixtureRepeatIndex == -1
                    ? string.Empty
                    : string.Format("_{0}_{1}", TestFixtureRepeatIndex + 1, TestRepeatIndex + 1));

            ErrorMessage = errorMessage;
            StackTrace = stackTrace;
            TimeTaken = timeTaken;
            Version = version;
            Authors = authors;
            CreateDateTime = DateTime.Now;
        }

        #endregion

    }
}
