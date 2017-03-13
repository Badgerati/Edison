/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Engine.Models;
using Edison.Engine.Repositories.Interfaces;
using Edison.Framework;
using Edison.Injector;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Edison.Engine.Utilities.Helpers
{
    public static class TestResultUrlHelper
    {

        #region Repositories

        private static IWebRequestRepository WebRequestRepository
        {
            get { return DIContainer.Instance.Get<IWebRequestRepository>(); }
        }

        #endregion

        #region Fields

        private static string _sessionId = null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends the test result callout.
        /// </summary>
        /// <param name="result">The test result to send.</param>
        /// <param name="context">The main context.</param>
        public static void SendTestResult(TestResult result, EdisonContext context)
        {
            if (result == default(TestResult) || context ==  default(EdisonContext))
            {
                return;
            }

            // build the value to send to the URL
            var urlModel = new TestResultUrlModel()
            {
                TestRunId = context.TestRunId,
                TestRunName = context.TestRunName,
                TestRunProject = context.TestRunProject,
                TestRunEnvironment = context.TestRunEnvironment,
                SessionId = _sessionId,
                Action = TestResultUrlActionType.Result,
                TestResults = new TestResultModel[] { new TestResultModel(result) }
            };

            // serialize for the JSON value
            var value = JsonConvert.SerializeObject(urlModel);

            // attempt to send the result to the URL
            SendCallout(value, context);
        }

        /// <summary>
        /// Sends the start event callout.
        /// </summary>
        /// <param name="context">The main context.</param>
        public static void SendStart(EdisonContext context)
        {
            if (context == default(EdisonContext))
            {
                return;
            }

            // build the value to send to the URL
            var urlModel = new TestResultUrlModel()
            {
                TestRunId = context.TestRunId,
                TestRunName = context.TestRunName,
                TestRunProject = context.TestRunProject,
                TestRunEnvironment = context.TestRunEnvironment,
                SessionId = null,
                Action = TestResultUrlActionType.Start,
                TestResults = null
            };

            // serialize for the JSON value
            var value = JsonConvert.SerializeObject(urlModel);

            // attempt to send the start event to the URL, and store the sessionId returned
            _sessionId = SendCallout(value, context);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attempts to send the callout data.
        /// </summary>
        /// <param name="value">The data to send.</param>
        /// <param name="context">The main context.</param>
        /// <returns>The response from the endpoint (normally just a SessionId)</returns>
        private static string SendCallout(string data, EdisonContext context)
        {
            // if the value is empty, return
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }

            // attempt to send the callout
            var request = WebRequestRepository.Create(context.TestResultURL);
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Timeout = 10000;

            var bytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            // Send the data, and read back for a response
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        #endregion

    }
}
