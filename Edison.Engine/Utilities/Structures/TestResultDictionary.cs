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
using System.Text;
using Edison.Engine.Events;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using Edison.Engine.Repositories.Outputs;

namespace Edison.Engine.Utilities.Structures
{
    [Serializable]
    public class TestResultDictionary
    {

        #region Repositories

        private IWebRequestRepository WebRequestRepository
        {
            get { return DIContainer.Instance.Get<IWebRequestRepository>(); }
        }

        #endregion

        #region Properties

        private ConcurrentDictionary<string, TestResult> Results = default(ConcurrentDictionary<string, TestResult>);
        private EdisonContext Context = default(EdisonContext);
        public event TestResultEventHandler OnTestResult;

        public ICollection<TestResult> TestResults
        {
            get { return Results.Values; }
        }

        public IEnumerable<TestResult> FailedTestResults
        {
            get { return Results.Values.Where(x => x.AbsoluteState != TestResultAbsoluteState.Success); }
        }

        public IEnumerable<TestResult> SuccessTestResults
        {
            get { return Results.Values.Where(x => x.AbsoluteState == TestResultAbsoluteState.Success); }
        }

        public double SuccessRate
        {
            get { return Math.Round(((double)SuccessCount / (double)TotalCount) * 100.0, 1); }
        }

        public double FailureRate
        {
            get { return Math.Round(((double)TotalFailedCount / (double)TotalCount) * 100.0, 1); }
        }

        public int TotalCount
        {
            get { return Results.Count; }
        }

        public int TotalFailedCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Failure
                                         || x.Value.AbsoluteState == TestResultAbsoluteState.Error); }
        }

        public int SuccessCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Success); }
        }

        public int FailureCount
        {
            get
            {
                return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Failure);
            }
        }

        public int ErrorCount
        {
            get
            {
                return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Error);
            }
        }

        public int IgnoredCount
        {
            get
            {
                return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Ignored);
            }
        }

        public int InconclusiveCount
        {
            get
            {
                return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Inconclusive);
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
            var key = result.Assembly + "." + result.FullName;

            if (Results.TryGetValue(key, out _result))
            {
                if (_result != default(TestResult) && _result.State != TestResultState.Success)
                {
                    response = Results.TryUpdate(key, result, _result);
                }
            }
            else
            {
                response = Results.TryAdd(key, result);
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

            if (OnTestResult != default(TestResultEventHandler))
            {
                OnTestResult.Invoke(result);
            }

            return response;
        }

        public TestResult Get(string testName)
        {
            var _result = default(TestResult);
            Results.TryGetValue(testName, out _result);
            return _result;
        }

        public string ToTotalString()
        {
            return string.Format("Total: {1}, Passed: {2}, Failed: {3}, Errored: {4}, Inconclusive: {5}, Skipped: {6}{0}Success Rate: {7}%{0}Failure Rate: {8}%",
                Environment.NewLine,
                TotalCount,
                SuccessCount,
                FailureCount,
                ErrorCount,
                InconclusiveCount,
                IgnoredCount,
                SuccessRate,
                FailureRate);
        }

        #endregion

        #region Private Methods

        private void PostResultToUrl(TestResult result)
        {
            if (string.IsNullOrWhiteSpace(Context.TestResultURL))
            {
                return;
            }

            var output = OutputRepositoryFactory.Get(Context.OutputType);
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

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var request = WebRequestRepository.Create(Context.TestResultURL + "?TestRunId=" + StringExtension.Safeguard(Context.TestRunId).ToUrlString());
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
