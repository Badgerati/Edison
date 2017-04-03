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
    public class CsvOutputRepository : IOutputRepository
    {

        private static Lazy<CsvOutputRepository> _lazy = new Lazy<CsvOutputRepository>(() => new CsvOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.Csv; }
        }

        public string ContentType
        {
            get { return "application/x-www-form-urlencoded"; }
        }

        public string OpenTag
        {
            get { return "\"Test\", \"State\", \"TimeTaken\", \"ErrorMessage\", \"StackTrace\", \"CreateDate\", \"Assembly\""; }
        }

        public string CloseTag
        {
            get { return string.Empty; }
        }

        public string Extension
        {
            get { return "csv"; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            return string.Format("\"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\", \"{7}\"{0}",
                withTrail ? Environment.NewLine : string.Empty,
                RemoveNewLines(result.FullName),
                result.State,
                result.Duration,
                RemoveNewLines(result.ErrorMessage),
                RemoveNewLines(result.StackTrace),
                result.CreateDateTimeString,
                result.Assembly);
        }


        private string RemoveNewLines(string value)
        {
            return value.Replace(Environment.NewLine, " ").Replace("\r", " ").Replace("\r\n", " ").Replace('"', '\'').Trim();
        }

    }
}
