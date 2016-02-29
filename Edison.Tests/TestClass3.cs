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
    public class TestClass3
    {

        [Test]
        [Repeat(2)]
        public void TestMethod()
        {
            Console.WriteLine("WOO7!!!");
            Console.WriteLine(TestStatisClass.StringValue);
            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
            Console.WriteLine("WOO4!!!");
        }

    }
}
