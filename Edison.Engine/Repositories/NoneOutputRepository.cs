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

namespace Edison.Engine.Repositories
{
    public class NoneOutputRepository : IOutputRepository
    {

        private static Lazy<NoneOutputRepository> _lazy = new Lazy<NoneOutputRepository>(() => new NoneOutputRepository());
        public static IOutputRepository Instance
        {
            get { return _lazy.Value; }
        }


        public OutputType OutputType
        {
            get { return OutputType.None; }
        }

        public string ContentType
        {
            get { return string.Empty; }
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
            return string.Empty;
        }

    }
}
