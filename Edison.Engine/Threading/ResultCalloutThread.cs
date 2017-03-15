/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Utilities.Helpers;
using Edison.Framework;
using Edison.Injector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Edison.Engine.Threading
{
    public class ResultCalloutThread
    {

        #region Repositories

        private IWebRequestRepository WebRequestRepository
        {
            get { return DIContainer.Instance.Get<IWebRequestRepository>(); }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of items on this queue.
        /// </summary>
        public int Count
        {
            get { return _queue.Count; }
        }

        #endregion

        #region Fields

        private EdisonContext _context = default(EdisonContext);
        private ConcurrentQueue<Tuple<TestResult, ResultCalloutType>> _queue = default(ConcurrentQueue<Tuple<TestResult, ResultCalloutType>>);
        private bool _interrupted = false;
        private Dictionary<ResultCalloutType, int> _blackList = default(Dictionary<ResultCalloutType, int>);

        private bool _canPostToSlack = false;
        private bool _canPostToResultUrl = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of this callout thread.
        /// </summary>
        /// <param name="context">The main EdisonContext to help setup the thread.</param>
        public ResultCalloutThread(EdisonContext context)
        {
            _context = context;
            _queue = new ConcurrentQueue<Tuple<TestResult, ResultCalloutType>>();
            _blackList = new Dictionary<ResultCalloutType, int>();

            _canPostToSlack = !string.IsNullOrWhiteSpace(context.SlackToken);
            _canPostToResultUrl = !string.IsNullOrWhiteSpace(context.TestResultURL);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a result to the thread's queue.
        /// </summary>
        /// <param name="result">The test result to add.</param>
        /// <param name="type">The callout type to send the result to.</param>
        public void Add(TestResult result, ResultCalloutType type)
        {
            _queue.Enqueue(Tuple.Create(result, type));
        }

        /// <summary>
        /// Interrupt the thread to stop it.
        /// </summary>
        public void Interrupt()
        {
            _interrupted = true;
        }

        /// <summary>
        /// Run the main loop logic to send out callouts.
        /// </summary>
        public void Run()
        {
            while (!(_interrupted && Count == 0))
            {
                // if there's nothing to send, sleep for a little
                if (!_queue.Any())
                {
                    Thread.Sleep(500);
                    continue;
                }

                //  try and get the first item, on fail continue after mini sleep
                var item = default(Tuple<TestResult, ResultCalloutType>);
                if (!_queue.TryDequeue(out item))
                {
                    Thread.Sleep(200);
                    continue;
                }

                // ensure the item is not null
                if (item == default(Tuple<TestResult, ResultCalloutType>))
                {
                    Thread.Sleep(200);
                    continue;
                }

                // ensure the callout type is not blacklisted
                if (IsBlackListed(item.Item2))
                {
                    continue;
                }

                // attempt to send the callout
                switch (item.Item2)
                {
                    case ResultCalloutType.Slack:
                        PostResultToSlack(item.Item1);
                        break;

                    case ResultCalloutType.TestResultUrl:
                        PostResultToResultUrl(item.Item1);
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sends a test's result to slack.
        /// </summary>
        /// <param name="result">The test result.</param>
        private void PostResultToSlack(TestResult result)
        {
            // if there's no slack token, or result is not slackable, return
            if (!_canPostToSlack || !result.IsSlackable)
            {
                return;
            }

            try
            {
                // attempt to send the test result
                SlackHelper.SendMessage(result, _context.SlackToken);
            }
            catch (Exception ex)
            {
                IncrementBlackList(ResultCalloutType.Slack);

                #if DEBUG
                {
                    Logger.Instance.WriteError(string.Format("Failed posting result to Slack:\n{0}", ex.Message));
                }
                #endif
            }
        }

        /// <summary>
        /// Sends a test's result to the test result URL.
        /// </summary>
        /// <param name="result">The test result.</param>
        private void PostResultToResultUrl(TestResult result)
        {
            // if there is no Result URL, return
            if (!_canPostToResultUrl)
            {
                return;
            }

            try
            {
                // attempt to send the test result
                TestResultUrlHelper.SendTestResult(result, _context);
            }
            catch (Exception ex)
            {
                IncrementBlackList(ResultCalloutType.TestResultUrl);

                #if DEBUG
                {
                    Logger.Instance.WriteError(string.Format("Failed posting result to TestResultURL:\n{0}", ex.Message));
                }
                #endif
            }
        }

        /// <summary>
        /// Increment the callout type on the blacklist if a callout fails.
        /// </summary>
        /// <param name="type">Callout type to increment.</param>
        private void IncrementBlackList(ResultCalloutType type)
        {
            if (_blackList.ContainsKey(type))
            {
                _blackList[type]++;
            }
            else
            {
                _blackList.Add(type, 1);
            }
        }

        /// <summary>
        /// Returns whether or not a callout type is blacklisted (aka, failed 5 times or more).
        /// </summary>
        /// <param name="type">Callout type to check.</param>
        /// <returns>True if blacklisted, false otherwise.</returns>
        private bool IsBlackListed(ResultCalloutType type)
        {
            return _blackList.ContainsKey(type) && _blackList[type] >= 5;
        }

        #endregion

    }
}
