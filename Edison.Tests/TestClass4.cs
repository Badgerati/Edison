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
    [TestCase("1", 1, true)]
    [TestCase("2", 2, false)]
    public class TestClass4
    {

        private string Value1;
        private int Value2;
        private bool Value3;

        public TestClass4(string value1, int value2, bool value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }

        [Test]
        public void TestMethod()
        {
            Console.WriteLine("Value1: " + Value1 + ", Value2: " + Value2 + ", Value3: " + Value3);
            AssertFactory.Instance.AreEqual(Value1, Value2.ToString(), "Values are not equal");
        }

    }
}
