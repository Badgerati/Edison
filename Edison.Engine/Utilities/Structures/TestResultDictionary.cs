/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Edison.Engine.Events;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using Edison.Engine.Utilities.Helpers;

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

        /// <summary>
        /// Gets the test results.
        /// </summary>
        /// <value>
        /// The test results.
        /// </value>
        public ICollection<TestResult> TestResults
        {
            get { return Results.Values; }
        }

        /// <summary>
        /// Gets the failed test results.
        /// </summary>
        /// <value>
        /// The failed test results.
        /// </value>
        public IEnumerable<TestResult> FailedTestResults
        {
            get { return Results.Values.Where(x => x.AbsoluteState != TestResultAbsoluteState.Success); }
        }

        /// <summary>
        /// Gets the success test results.
        /// </summary>
        /// <value>
        /// The success test results.
        /// </value>
        public IEnumerable<TestResult> SuccessTestResults
        {
            get { return Results.Values.Where(x => x.AbsoluteState == TestResultAbsoluteState.Success); }
        }

        /// <summary>
        /// Gets the success rate.
        /// </summary>
        /// <value>
        /// The success rate.
        /// </value>
        public double SuccessRate
        {
            get { return Math.Round(((double)SuccessCount / (double)TotalCount) * 100.0, 1); }
        }

        /// <summary>
        /// Gets the failure rate.
        /// </summary>
        /// <value>
        /// The failure rate.
        /// </value>
        public double FailureRate
        {
            get { return Math.Round(((double)TotalFailedCount / (double)TotalCount) * 100.0, 1); }
        }

        /// <summary>
        /// Gets the total test result count.
        /// </summary>
        /// <value>
        /// The total test result count.
        /// </value>
        public int TotalCount
        {
            get { return Results.Count; }
        }

        /// <summary>
        /// Gets the total failed result count.
        /// </summary>
        /// <value>
        /// The total failed count.
        /// </value>
        public int TotalFailedCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Failure
                                         || x.Value.AbsoluteState == TestResultAbsoluteState.Error); }
        }

        /// <summary>
        /// Gets the success result count.
        /// </summary>
        /// <value>
        /// The success count.
        /// </value>
        public int SuccessCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Success); }
        }

        /// <summary>
        /// Gets the failure count.
        /// </summary>
        /// <value>
        /// The failure count.
        /// </value>
        public int FailureCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Failure); }
        }

        /// <summary>
        /// Gets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        public int ErrorCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Error); }
        }

        /// <summary>
        /// Gets the ignored count.
        /// </summary>
        /// <value>
        /// The ignored count.
        /// </value>
        public int IgnoredCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Ignored); }
        }

        /// <summary>
        /// Gets the inconclusive count.
        /// </summary>
        /// <value>
        /// The inconclusive count.
        /// </value>
        public int InconclusiveCount
        {
            get { return Results.Count(x => x.Value.AbsoluteState == TestResultAbsoluteState.Inconclusive); }
        }

        #endregion

        #region Fields

        private bool _canPostToSlack = false;
        private bool _canPostToUrl = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TestResultDictionary"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public TestResultDictionary(EdisonContext context)
        {
            Results = new ConcurrentDictionary<string,TestResult>();
            Context = context;

            _canPostToSlack = !string.IsNullOrWhiteSpace(context.SlackToken);
            _canPostToUrl = !string.IsNullOrWhiteSpace(context.TestResultURL);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds or update a test result within the dictionary.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>True if added/updated successfully, false otherwise.</returns>
        public bool AddOrUpdate(TestResult result)
        {
            var response = false;
            var key = result.Assembly + "." + result.FullName;

            // If a result exists for the test, update it. Else add the new result
            if (Results.TryGetValue(key, out var _result))
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

            // Log the test result to the console
            if (Logger.Instance.ConsoleOutputType != OutputType.None)
            {
                lock (this)
                {
                    Logger.Instance.WriteTestResult(result);
                }
            }

            // Attempt to send the result to a URL
            PostResultToUrl(result);

            // Attempt to send the result to Slack
            PostResultToSlack(result);

            // Invoke the callback handler event
            if (OnTestResult != default(TestResultEventHandler))
            {
                OnTestResult.Invoke(result);
            }

            return response;
        }

        /// <summary>
        /// Gets the specified test result by test name.
        /// </summary>
        /// <param name="testName">Name of the test.</param>
        /// <returns>The test result if one exists.</returns>
        public TestResult Get(string testName)
        {
            Results.TryGetValue(testName, out var _result);
            return _result;
        }

        /// <summary>
        /// Returns this instance's test results as a string.
        /// </summary>
        /// <returns>The test results as a string.</returns>
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

        private void PostResultToSlack(TestResult result)
        {
            // if there's not slack token, or result is not slackable, return
            if (!_canPostToSlack || !result.IsSlackable)
            {
                return;
            }

            try
            {
                // attempt to send the test result
                SlackHelper.SendMessage(result, Context.SlackToken);
            }
            catch (Exception ex)
            {
                #if DEBUG
                {
                    Logger.Instance.WriteError(string.Format("Failed posting result to Slack:\n{0}", ex.Message));
                }
                #endif
            }
        }

        /// <summary>
        /// Posts the result to a URL.
        /// </summary>
        /// <param name="result">The test result.</param>
        private void PostResultToUrl(TestResult result)
        {
            // if there is not URL, return
            if (!_canPostToUrl)
            {
                return;
            }

            try
            {
                // attempt to send the test result
                TestResultUrlHelper.SendTestResult(result, Context);
            }
            catch (Exception ex)
            {
                #if DEBUG
                {
                    Logger.Instance.WriteError(string.Format("Failed posting result to TestResultURL:\n{0}", ex.Message));
                }
                #endif
            }
        }

        #endregion

    }
}
