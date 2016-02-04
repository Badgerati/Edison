/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Framework;
using Edison.Engine.Utilities.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Edison.Engine.Core.Output;
using Edison.Framework.Enums;

namespace Edison.Engine.Utilities.Structures
{
    public class TestResultDictionary
    {

        #region Properties

        private ConcurrentDictionary<string, TestResult> Results = default(ConcurrentDictionary<string, TestResult>);
        private EdisonContext Context = default(EdisonContext);

        public ICollection<TestResult> TestResults
        {
            get { return Results.Values; }
        }

        public IEnumerable<TestResult> FailedTestResults
        {
            get { return Results.Values.Where(x => x.State != TestResultState.Success); }
        }

        public IEnumerable<TestResult> SuccessTestResults
        {
            get { return Results.Values.Where(x => x.State == TestResultState.Success); }
        }

        public int Total
        {
            get { return Results.Count; }
        }

        public int Passed
        {
            get { return Count(TestResultState.Success); }
        }

        public int Failed
        {
            get
            {
                return Count(TestResultState.Failure)
                    + Count(TestResultState.GlobalSetupFailure)
                    + Count(TestResultState.GlobalTeardownFailure)
                    + Count(TestResultState.SetupFailure)
                    + Count(TestResultState.TeardownFailure)
                    + Count(TestResultState.TestFixtureSetupFailure)
                    + Count(TestResultState.TestFixtureTeardownFailure);
            }
        }

        public int Errored
        {
            get
            {
                return Count(TestResultState.Error)
                    + Count(TestResultState.GlobalSetupError)
                    + Count(TestResultState.GlobalTeardownError)
                    + Count(TestResultState.SetupError)
                    + Count(TestResultState.TeardownError)
                    + Count(TestResultState.TestFixtureSetupError)
                    + Count(TestResultState.TestFixtureTeardownError);
            }
        }

        public int Skipped
        {
            get
            {
                return Count(TestResultState.Ignored);
            }
        }

        public int Inconclusive
        {
            get
            {
                return Count(TestResultState.Inconclusive);
            }
        }

        #endregion

        #region Constructor

        public TestResultDictionary(EdisonContext context)
        {
            Results = new ConcurrentDictionary<string,TestResult>();
            Context = context;
        }

        #endregion

        #region Public Methods

        public bool AddOrUpdate(TestResult result)
        {
            var response = false;
            var _result = default(TestResult);

            if (Results.TryGetValue(result.Name, out _result))
            {
                if (_result != default(TestResult) && _result.State != TestResultState.Success)
                {
                    response = Results.TryUpdate(result.Name, result, _result);
                }
            }
            else
            {
                response = Results.TryAdd(result.Name, result);
            }

            if (Logger.Instance.ConsoleOutputType != OutputType.None)
            {
                lock (this)
                {
                    Logger.Instance.WriteTestResult(result);
                    Logger.Instance.WriteSingleLine(Environment.NewLine, Environment.NewLine);
                }
            }

            try
            {
                PostResultToUrl(result);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError(string.Format("Failed posting result to TestResultURL:\n{0}", ex.Message));
            }

            return response;
        }

        public TestResult Get(string testName)
        {
            var _result = default(TestResult);
            Results.TryGetValue(testName, out _result);
            return _result;
        }

        public int Count(TestResultState state)
        {
            return Results.Count(x => x.Value.State == state);
        }

        public string ToTotalString()
        {
            return string.Format("Total: {0}, Passed: {1}, Failed: {2}, Errored: {3}, Inconclusive: {4}, Skipped: {5}",
                Total,
                Passed,
                Failed,
                Errored,
                Inconclusive,
                Skipped);
        }

        #endregion

        #region Private Methods

        private void PostResultToUrl(TestResult result)
        {
            if (string.IsNullOrEmpty(Context.TestResultURL))
            {
                return;
            }

            var output = OutputRepositoryManager.Get(Context.OutputType);
            var value = string.Empty;

            switch (Context.OutputType)
            {
                case OutputType.Csv:
                    value = output.OpenTag + Environment.NewLine + output.ToString(result, false);
                    break;

                default:
                    value = output.ToString(result, false);
                    break;
            }

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var request = (WebRequest)HttpWebRequest.Create(Context.TestResultURL + "?TestRunId=" + StringExtension.Safeguard(Context.TestRunId).ToUrlString());
            request.Method = "POST";
            request.ContentType = output.ContentType;

            var bytes = Encoding.ASCII.GetBytes(value);
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            using (var response = request.GetResponse())
            {
                Logger.Instance.WriteMessage("Result posted to TestResultURL");
            }
        }

        #endregion

    }
}
