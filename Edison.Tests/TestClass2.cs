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
    [Suite("SuiteTests")]
    public class TestClass2
    {

        [Test]
        [Repeat(2)]
        public void TestMethod()
        {
            Console.WriteLine("WOO8!!!");
            Console.WriteLine(TestStatisClass.StringValue);
            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
            Console.WriteLine("WOO9!!!");
        }

        [Test]
        public void TestListFailure()
        {
            var values1 = new[] { 1, 2, 3 };
            var values2 = new[] { 1, 2, 4 };
            AssertFactory.Instance.AreEnumerablesEqual(values1, values2);
        }

    }
}
