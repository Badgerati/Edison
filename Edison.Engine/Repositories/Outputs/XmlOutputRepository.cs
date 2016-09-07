/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Edison.Engine.Repositories.Interfaces;
using Edison.Framework;
using Edison.Engine.Utilities.Extensions;
using System;

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
            return string.Format("<TestResult>{0}<Test>{2}</Test>{0}<State>{3}</State>{0}<TimeTaken>{4}</TimeTaken>{0}<ErrorMessage>{5}</ErrorMessage>{0}<StackTrace>{6}</StackTrace>{0}<CreateDate>{7}</CreateDate>{0}<Assembly>{8}</Assembly>{0}</TestResult>{1}",
                Environment.NewLine,
                withTrail ? Environment.NewLine : string.Empty,
                result.FullName.ToHtmlString(),
                result.State,
                result.TimeTaken,
                result.ErrorMessage.ToHtmlString(),
                result.StackTrace.ToHtmlString(),
                result.CreateDateTimeString.ToHtmlString(),
                result.Assembly.ToHtmlString());
        }

    }
}
