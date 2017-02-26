/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Repositories.Outputs;
using Edison.Framework;
using Edison.Injector;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Edison.Engine.Utilities.Helpers
{
    public static class SlackHelper
    {

        #region Repositories

        private static IWebRequestRepository WebRequestRepository
        {
            get { return DIContainer.Instance.Get<IWebRequestRepository>(); }
        }

        private static IOutputRepository OutputRepository = new TxtOutputRepository();

        #endregion

        #region Constants

        private const string SlackUrl = "https://slack.com/api/chat.postMessage";
        private const string IconUrl = "https://cdn.rawgit.com/Badgerati/Edison/master/Images/icon.png";
        private const string DataFormat = "token={0}&channel={1}&attachments=[{2}]&link_names=1&as_user=false&username=Edison&icon_url={3}";

        #endregion


        /// <summary>
        /// Sends the test result to slack.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="token">The token.</param>
        public static void SendMessage(TestResult result, string token)
        {
            // create the attachments fields
            var fieldList = new List<SlackField>();

            fieldList.Add(new SlackField()
            {
                title = "Test Name",
                @short = false,
                value = result.TestName
            });

            fieldList.Add(new SlackField()
            {
                title = "Result",
                @short = true,
                value = result.State.ToString()
            });

            fieldList.Add(new SlackField()
            {
                title = "Duration",
                @short = true,
                value = result.TimeTaken.ToString("h'h 'm'm 's's 'fff'ms'")
            });

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            {
                fieldList.Add(new SlackField()
                {
                    title = "Error Message",
                    @short = false,
                    value = result.ErrorMessage
                });

                fieldList.Add(new SlackField()
                {
                    title = "Stack Trace",
                    @short = false,
                    value = result.StackTrace
                });
            }

            // get colour of message from result
            var colour = GetColour(result.AbsoluteState);

            // create attachment
            var attachment = new SlackAttachment()
            {
                fallback = OutputRepository.ToString(result, false),
                color = colour,
                pretext = string.Empty,
                title = string.Empty,
                fields = fieldList.ToArray()
            };

            // serialise the attachment to json
            var attachmentJson = JsonConvert.SerializeObject(attachment);
            var data = string.Format(DataFormat, token, result.SlackChannel, attachmentJson, IconUrl);

            // attempt to post result to slack
            var request = WebRequestRepository.Create(SlackUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var bytes = Encoding.ASCII.GetBytes(data);
            request.ContentLength = bytes.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            // Log that the result was sent
            using (var response = request.GetResponse())
            {
                #if DEBUG
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        Logger.Instance.WriteMessage(reader.ReadToEnd());
                    }
                }
                #endif
            }
        }

        /// <summary>
        /// Gets the colour for the slack attachment based on the test's result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The colour for the slack attachment.</returns>
        private static string GetColour(TestResultAbsoluteState result)
        {
            switch (result)
            {
                case TestResultAbsoluteState.Error:
                    return "danger";

                case TestResultAbsoluteState.Failure:
                    return "warning";

                case TestResultAbsoluteState.Success:
                    return "good";

                default:
                    return "#439FE0";
            }
        }

    }

    #region Slack Objects

    public class SlackAttachment
    {
        public string fallback { get; set; }
        public string color { get; set; }
        public string pretext { get; set; }
        public string title { get; set; }
        public SlackField[] fields { get; set; }
    }

    public class SlackField
    {
        public string title { get; set; }
        public string value { get; set; }
        public bool @short { get; set; }
    }

    #endregion

}
