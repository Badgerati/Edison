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

namespace Edison.Engine.Repositories.Outputs
{
    public class JsonOutputRepository : IOutputRepository
    {

        private static Lazy<JsonOutputRepository> _lazy = new Lazy<JsonOutputRepository>(() => new JsonOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.Json; }
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

        public string Extension
        {
            get { return "json"; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            return string.Format("{{\"test\":\"{1}\", \"state\":\"{2}\", \"timetaken\":\"{3}\", \"errormessage\":\"{4}\", \"stacktrace\":\"{5}\", \"createdate\":\"{6}\", \"assembly\":\"{7}\"}}{0}",
                withTrail ? ("," + Environment.NewLine) : string.Empty,
                JsonFormat(result.FullName),
                result.State,
                result.TimeTaken,
                JsonFormat(result.ErrorMessage),
                JsonFormat(result.StackTrace),
                JsonFormat(result.CreateDateTimeString),
                JsonFormat(result.Assembly));
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
