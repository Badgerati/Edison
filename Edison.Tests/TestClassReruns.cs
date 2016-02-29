/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;

namespace Edison.Tests
{
    [TestFixture]
    public class TestClassReruns
    {

        [Test]
        public void TestMethod()
        {
            TestStatisClass.IntValue++;
            AssertFactory.Instance.AreEqual(2, TestStatisClass.IntValue, "Values are not equal");
        }

    }
}
