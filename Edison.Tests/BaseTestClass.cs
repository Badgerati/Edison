/*
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

namespace Edison.Tests
{
    public class BaseTestClass
    {

        [Setup]
        public void BaseSetup()
        {
            Console.WriteLine("This is the base setup");
        }

        [Teardown]
        public void BaseTeardown(TestResult result)
        {
            Console.WriteLine("This is the base teardown");
        }

    }
}
