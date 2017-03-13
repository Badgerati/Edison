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

        public void Add(TestResult result, ResultCalloutType type)
        {
            _queue.Enqueue(Tuple.Create(result, type));
        }

        public void Interrupt()
        {
            _interrupted = true;
        }

        public void Run()
        {
            while (!(_interrupted && Count == 0))
            {
                // if there's nothing to send, sleep for a little
                if (!_queue.Any())
                {
                    Thread.Sleep(400);
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

        private bool IsBlackListed(ResultCalloutType type)
        {
            return _blackList.ContainsKey(type) && _blackList[type] >= 5;
        }

        #endregion

    }
}
