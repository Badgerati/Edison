/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Framework
{
    public class AssertFactory
    {

        #region Properties

        private static Lazy<AssertFactory> _lazy = new Lazy<AssertFactory>(() => new AssertFactory());
        public static IAssert Instance
        {
            get { return _lazy.Value.AssertMethod; }
            set { _lazy.Value.AssertMethod = value; }
        }

        public IAssert AssertMethod = default(IAssert);

        #endregion

        #region Constructor

        private AssertFactory()
        {
            AssertMethod = new Assert();
        }

        #endregion

    }
}
