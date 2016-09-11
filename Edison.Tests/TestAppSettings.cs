/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;
using System.Configuration;

namespace Edison.Tests
{
    [TestFixture]
    public class TestAppSettings
    {

        [Test]
        public void TestAppSettingTestValue()
        {
            var value = ConfigurationManager.AppSettings["TestKey"];
            AssertFactory.Instance.AreEqual("TestValue123", value);
        }

        [Test]
        public void TestAppSettingTestValue2()
        {
            var value = ConfigurationManager.AppSettings["TestKey2"];
            AssertFactory.Instance.AreEqual("TestValue1234", value);
        }

    }
}
