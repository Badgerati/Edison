﻿/*
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
    public class CsvOutputRepository : IOutputRepository
    {

        private static Lazy<CsvOutputRepository> _lazy = new Lazy<CsvOutputRepository>(() => new CsvOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public string ContentType
        {
            get { return "application/x-www-form-urlencoded"; }
        }

        public string OpenTag
        {
            get { return "\"Test\", \"State\", \"TimeTaken\", \"ErrorMessage\", \"StackTrace\", \"CreateDate\""; }
        }

        public string CloseTag
        {
            get { return string.Empty; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            return string.Format("\"{1}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\", \"{6}\"{0}",
                withTrail ? Environment.NewLine : string.Empty,
                RemoveNewLines(result.Name),
                result.State,
                result.TimeTaken,
                RemoveNewLines(result.ErrorMessage.Replace("Error Message: ", string.Empty)),
                RemoveNewLines(result.StackTrace),
                RemoveNewLines(result.CreateDateTimeString));
        }


        private string RemoveNewLines(string value)
        {
            return value.Replace(Environment.NewLine, " ").Replace("\r", " ").Replace("\r\n", " ").Replace('"', '\'').Trim();
        }

    }
}
