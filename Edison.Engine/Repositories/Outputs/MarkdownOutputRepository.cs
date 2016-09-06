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
    public class MarkdownOutputRepository : IOutputRepository
    {

        private static Lazy<MarkdownOutputRepository> _lazy = new Lazy<MarkdownOutputRepository>(() => new MarkdownOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.Markdown; }
        }

        public string ContentType
        {
            get { return "text/markdown"; }
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
            get { return "md"; }
        }


        public string ToString(TestResult result, bool withTrail)
        {
            return result.State != TestResultState.Success
                ? string.Format("### {1}{0}```{0}Assembly: {2}{0}State: {3}{0}Time Taken: {4}{0}Create Date: {5}{0}{0}{6}{0}{0}StackTrace:{0}{7}{0}```{0}{0}",
                    Environment.NewLine,
                    result.FullName,
                    result.Assembly,
                    result.State,
                    result.TimeTaken,
                    result.CreateDateTimeString,
                    result.ErrorMessage,
                    result.StackTrace)
                : string.Format("### {1}{0}```{0}Assembly: {2}{0}State: {3}{0}Time Taken: {4}{0}Create Date: {5}{0}```{0}{0}",
                    Environment.NewLine,
                    result.FullName,
                    result.Assembly,
                    result.State,
                    result.TimeTaken,
                    result.CreateDateTimeString,
                    result.ErrorMessage,
                    result.StackTrace);
        }

    }
}
