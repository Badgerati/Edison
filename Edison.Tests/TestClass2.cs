/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Tests
{
    [TestFixture]
    //[Ignore]
    public class TestClass2
    {

        [Test]
        [Repeat(2)]
        public void TestMethod()
        {
            Console.WriteLine("WOO8!!!");
            Console.WriteLine(TestStatisClass.Value);
            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
            Console.WriteLine("WOO9!!!");
        }

    }
}
