﻿/*
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
    [Suite("OtherSuite")]
    //[Category("eek")]
    [Repeat(2)]
    public class TestClass : BaseTestClass
    {

        private string test = "this didn't work...";

        [Setup]
        public void Setup()
        {
            Console.WriteLine("This is the setup");
            test = "Is this working?";
        }

        [Teardown]
        public void Teardown(TestResult result)
        {
            Console.WriteLine("This is the teardownz");
            test = "err?";
        }

        [TestFixtureSetup]
        public void FixtureSetup()
        {
            Console.WriteLine("This is the fixture setup");
        }

        [TestFixtureTeardown]
        public void FixtureTeardown()
        {
            Console.WriteLine("This is the fixture teardown");
        }



        [Test]
        [Repeat(2)]
        [TestCase("case1")]
        [TestCase("case2")]
        [Category("eek")]
        [ExpectedException(typeof(AssertException), AllowAssertException = true)]
        public void TestMethod1(string value)
        {
            Console.WriteLine("WOO1!!!");
            Console.WriteLine(value);
            Console.WriteLine(test);
            AssertFactory.Instance.AreEqual(1, 2, "Values are not equal");
            Console.WriteLine("WOO1!!!");
        }

        [Test]
        [Repeat(2)]
        [ExpectedException(typeof(AssertException), AllowAssertException = true)]
        public void TestMethod2()
        {
            Console.WriteLine("WOO2!!!");
            Console.WriteLine(test);
            Console.WriteLine(TestStatisClass.StringValue);
            AssertFactory.Instance.AreEqual(1, 2, "Values are not equal");
            Console.WriteLine("WOO2!!!");
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException), ExpectedMessage = "Object", MatchType = MatchType.EndsWith, InverseMatch = true)]
        public void TestMethod3()
        {
            Console.WriteLine("WOO3!!!");
            Console.WriteLine(test);

            string v = null;
            var len = v.Length;

            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
            Console.WriteLine("WOO3!!!");
        }

        [Test]
        public void TestMethod4()
        {
            AssertFactory.Instance.AreEqual(1 + 1, 2);
        }

    }
}
