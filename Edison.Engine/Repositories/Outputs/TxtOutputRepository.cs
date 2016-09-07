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
using System;

namespace Edison.Engine.Repositories.Outputs
{
    public class TxtOutputRepository : IOutputRepository
    {

        private static Lazy<TxtOutputRepository> _lazy = new Lazy<TxtOutputRepository>(() => new TxtOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.Txt; }
        }

        public string ContentType
        {
            get { return "application/x-www-form-urlencoded"; }
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
            get { return "txt"; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            return string.Format("Test: {2}{0}Assembly: {3}{0}State: {4}{0}Time Taken: {5}{0}Create Date: {6}{7}{1}",
                Environment.NewLine,
                withTrail ? Environment.NewLine + "= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" + Environment.NewLine : string.Empty,
                result.FullName,
                result.Assembly,
                result.State,
                result.TimeTaken,
                result.CreateDateTimeString,
                result.State == TestResultState.Success
                    ? string.Empty
                    : string.Format("{0}{0}Error Message: {1}{0}{0}StackTrace:{0}{2}",
                        Environment.NewLine,
                        result.ErrorMessage,
                        result.StackTrace));
        }

    }
}
