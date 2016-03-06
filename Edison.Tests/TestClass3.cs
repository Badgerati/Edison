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
    [Category("some category")]
    public class TestClass3
    {

        [Test]
        [Repeat(2)]
        [Category("hmm")]
        public void TestMethod()
        {
            Console.WriteLine("WOO7!!!");
            Console.WriteLine(TestStatisClass.StringValue);
            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
            Console.WriteLine("WOO4!!!");
        }

        [Test]
        [Repeat(2)]
        public void TestMethod2()
        {
            Thread.Sleep(1000);
            Console.WriteLine("WOO7!!!");
            Console.WriteLine(TestStatisClass.StringValue);
            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
            Console.WriteLine("WOO4!!!");
        }

        [Test]
        [ParallelRepeat(2)]
        public void TestMethod3()
        {
            Thread.Sleep(1000);
            Console.WriteLine("WOO7!!!");
            Console.WriteLine(TestStatisClass.StringValue);
            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
            Console.WriteLine("WOO4!!!");
        }

    }
}
