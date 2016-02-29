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
    [SetupFixture]
    public class GlobalTestClass
    {
        [Setup]
        public void Setup()
        {
            Console.WriteLine("This is the global setup");
            TestStatisClass.StringValue = "YAY!!!!";
        }

        [Teardown]
        public void Teardown()
        {
            Console.WriteLine("This is the global teardown");
        }


        private void DummyFail()
        {
            AssertFactory.Instance.AreEqual(1, 2);
        }

        private void DummyError()
        {
            string eek = null;
            eek.Replace("", "");
        }

    }
}
