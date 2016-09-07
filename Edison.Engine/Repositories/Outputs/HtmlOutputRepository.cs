/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Edison.Engine.Repositories.Interfaces;
using Edison.Framework;
using Edison.Framework.Enums;
using Edison.Engine.Utilities.Extensions;
using System;

namespace Edison.Engine.Repositories.Outputs
{
    public class HtmlOutputRepository : IOutputRepository
    {

        private static Lazy<HtmlOutputRepository> _lazy = new Lazy<HtmlOutputRepository>(() => new HtmlOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.Html; }
        }

        public string ContentType
        {
            get { return "text/html"; }
        }

        public string OpenTag
        {
            get { return string.Empty; }
        }

        public string CloseTag
        {
            get { return string.Empty; }
        }

        public string Extension
        {
            get { return "html"; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            return string.Format("<div class='test-result'><span class='test-name'><strong>{0}</strong></span><p class='test-details'><span class='assembly'><strong>Assembly</strong>: {1}</span><br/><span class='state'><strong>State</strong>: {2}</span><br/><span class='time-taken'><strong>Time Taken</strong>: {3}</span><br/><span class='create-date'><strong>Create Date</strong>: {4}</span></p>{5}</div><hr/>",
                result.FullName.ToHtmlString(),
                result.Assembly.ToHtmlString(),
                result.State,
                result.TimeTaken,
                result.CreateDateTimeString.ToHtmlString(),
                result.State == TestResultState.Success
                    ? string.Empty
                    : string.Format("<p class='error-message'><strong>Error Message</strong>:{0}<pre>{1}</pre></p><p class='stack-trace'><strong>StackTrace</strong>:{0}<pre>{2}</pre></p>",
                        Environment.NewLine,
                        result.ErrorMessage.ToHtmlString(),
                        result.StackTrace.ToHtmlString()));
        }

    }
}
