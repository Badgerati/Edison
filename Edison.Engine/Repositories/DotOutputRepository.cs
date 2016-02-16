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

namespace Edison.Engine.Repositories
{
    public class DotOutputRepository : IOutputRepository
    {

        private static Lazy<DotOutputRepository> _lazy = new Lazy<DotOutputRepository>(() => new DotOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.Dot; }
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
            var dot = "F";

            switch (result.State)
            {
                case TestResultState.Success:
                    dot = ".";
                    break;

                case TestResultState.Inconclusive:
                    dot = "I";
                    break;

                case TestResultState.Ignored:
                    dot = "S";
                    break;
            }

            return dot;
        }

    }
}
