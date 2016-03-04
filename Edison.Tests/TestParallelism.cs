/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Edison.Framework.Enums;
using System;
using System.Threading;

namespace Edison.Tests
{
    [TestFixture]
    [Suite("OtherSuite")]
    public class TestParallelism : BaseTestClass
    {

        [Test]
        public void TestMethod1()
        {
            Thread.Sleep(1000);
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod2()
        {
            Thread.Sleep(1000);
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod3()
        {
            Thread.Sleep(1000);
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod4()
        {
            Thread.Sleep(1000);
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

    }
}
