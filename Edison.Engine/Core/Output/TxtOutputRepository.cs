/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Edison.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine.Core.Output
{
    public class TxtOutputRepository : IOutputRepository
    {

        private static Lazy<TxtOutputRepository> _lazy = new Lazy<TxtOutputRepository>(() => new TxtOutputRepository());
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
            get { return string.Empty; }
        }

        public string CloseTag
        {
            get { return string.Empty; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            return result.State != TestResultState.Success
                ? string.Format("Test: {2}{0}State: {3}{0}Time Taken: {4}{0}Create Date: {7}{0}{0}Error Message: {5}{0}{0}StackTrace:{0}{6}{1}",
                    Environment.NewLine,
                    withTrail ? Environment.NewLine + "= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" + Environment.NewLine : string.Empty,
                    result.Name,
                    result.State,
                    result.TimeTaken,
                    result.ErrorMessage.Replace("Error Message: ", string.Empty),
                    result.StackTrace,
                    result.CreateDateTimeString)
                : string.Format("Test: {2}{0}State: {3}{0}Time Taken: {4}{0}Create Date: {5}{1}",
                    Environment.NewLine,
                    withTrail ? Environment.NewLine + "= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" + Environment.NewLine : string.Empty,
                    result.Name,
                    result.State,
                    result.TimeTaken,
                    result.CreateDateTimeString);
        }

    }
}
