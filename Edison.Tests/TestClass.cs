/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Edison.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Edison.Tests
{
    [TestFixture]
    //[Category("eek")]
    //[Repeat(2)]
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
        public void TestMethod1(string value)
        {
            Console.WriteLine("WOO1!!!");
            Console.WriteLine(value);
            Thread.Sleep(1000);
            Console.WriteLine(test);
            AssertFactory.Instance.AreEqual(1, 2, "Values are not equal");
            Console.WriteLine("WOO1!!!");
        }

        [Test]
        [Repeat(2)]
        public void TestMethod2()
        {
            //AssertFactory.Instance.Inconclusive("Hmm");
            Console.WriteLine("WOO2!!!");
            Console.WriteLine(test);
            Console.WriteLine(TestStatisClass.Value);
            AssertFactory.Instance.AreEqual(1, 2, "Values are not equal");
            Console.WriteLine("WOO2!!!");
        }

        [Test]
        [ExpectedException(typeof(NullReferenceException), "Object", MatchType.EndsWith, true)]
        public void TestMethod3()
        {
            Console.WriteLine("WOO3!!!");
            Console.WriteLine(test);

            string v = null;
            var len = v.Length;

            AssertFactory.Instance.AreEqual(1, 2, "Values are not equal");
            Console.WriteLine("WOO3!!!");
        }

        [Test]
        public void TestMethod4()
        {
            AssertFactory.Instance.AreEqual(1 + 1, 2);
        }

    }
}
