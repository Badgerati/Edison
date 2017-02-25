/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;

namespace Edison.Tests
{
    [TestFixture]
    public class TestSlack
    {

        [Test]
        [Slack("SOME_CHANNEL", SlackTestResult = SlackTestResultType.Any)]
        public void TestMethod1()
        {
            AssertFactory.Instance.AreEqual(1, 1, "Values are not equal");
        }

        [Test]
        [Slack("SOME_CHANNEL")]
        public void TestMethod2()
        {
            AssertFactory.Instance.AreEqual(1, 2, "Values are not equal");
        }

    }
}
