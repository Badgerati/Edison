/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Edison.Engine.Repositories.Interfaces;
using Edison.Framework;
using System;
using System.Web;

namespace Edison.Engine.Repositories.Outputs
{
    public class XmlOutputRepository : IOutputRepository
    {

        private static Lazy<XmlOutputRepository> _lazy = new Lazy<XmlOutputRepository>(() => new XmlOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.Xml; }
        }

        public string ContentType
        {
            get { return "text/xml; encoding='utf-8'"; }
        }

        public string OpenTag
        {
            get { return "<TestResults>"; }
        }

        public string CloseTag
        {
            get { return "</TestResults>"; }
        }

        public string Extension
        {
            get { return "xml"; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            var test = string.Format("<Test>{0}</Test>", ToHtmlString(result.FullName));
            var state = string.Format("<State>{0}</State>", result.State);
            var timeTaken = string.Format("<TimeTaken>{0}</TimeTaken>", result.TimeTaken);
            var message = string.Format("<ErrorMessage>{0}</ErrorMessage>", ToHtmlString(result.ErrorMessage.Replace("Error Message: ", string.Empty)));
            var stackTrace = string.Format("<StackTrace>{0}</StackTrace>", ToHtmlString(result.StackTrace));
            var createDate = string.Format("<CreateDate>{0}</CreateDate>", ToHtmlString(result.CreateDateTimeString));
            var assembly = string.Format("<Assembly>{0}</Assembly>", ToHtmlString(result.Assembly));

            return string.Format("<TestResult>{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}</TestResult>{1}",
                Environment.NewLine,
                withTrail ? Environment.NewLine : string.Empty,
                test,
                state,
                timeTaken,
                message,
                stackTrace,
                createDate,
                assembly);
        }


        private string ToHtmlString(string value)
        {
            return HttpUtility.HtmlEncode(value).Trim();
        }

    }
}
