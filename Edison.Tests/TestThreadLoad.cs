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
    [Concurrency(ConcurrencyType.Serial)]
    public class TestThreadLoad
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
        public void TestMethod4()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod5()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod6()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod7()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod8()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod9()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod10()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod11()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod12()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

        [Test]
        public void TestMethod13()
        {
            AssertFactory.Instance.AreEqual(2, 2, "Values are not equal");
        }

    }
}
