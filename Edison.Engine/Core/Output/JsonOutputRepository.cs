/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Core.Output
{
    public class JsonOutputRepository : IOutputRepository
    {

        private static Lazy<JsonOutputRepository> _lazy = new Lazy<JsonOutputRepository>(() => new JsonOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public string ContentType
        {
            get { return "text/json; encoding='utf-8'"; }
        }

        public string OpenTag
        {
            get { return "{\"testresults\":["; }
        }

        public string CloseTag
        {
            get { return "]}"; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            var test = string.Format("\"test\":\"{0}\"", JsonFormat(result.Name));
            var state = string.Format("\"state\":\"{0}\"", result.State);
            var timeTaken = string.Format("\"timetaken\":\"{0}\"", result.TimeTaken);
            var message = string.Format("\"errormessage\":\"{0}\"", JsonFormat(result.ErrorMessage.Replace("Error Message: ", string.Empty)));
            var stackTrace = string.Format("\"stacktrace\":\"{0}\"", JsonFormat(result.StackTrace));
            var createDate = string.Format("\"createdate\":\"{0}\"", JsonFormat(result.CreateDateTimeString));

            return string.Format("{{{2}, {3}, {4}, {5}, {6}, {7}}}{1}{0}",
                withTrail ? Environment.NewLine : string.Empty,
                withTrail ? "," : string.Empty,
                test,
                state,
                timeTaken,
                message,
                stackTrace,
                createDate);
        }

        private string RemoveNewLines(string value)
        {
            return value.Replace(Environment.NewLine, " ").Replace("\r", " ").Replace("\r\n", " ").Trim();
        }


        private string JsonFormat(string value)
        {
            return RemoveNewLines(value.Replace('"', '\'').Replace("\\", "/").Replace("\\\\", "/"));
        }

    }
}
