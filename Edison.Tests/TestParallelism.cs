/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
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
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod2()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod3()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        [Concurrency(ConcurrencyType.Serial)]
        public void TestMethod4()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

    }
}
