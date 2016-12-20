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
    public class TestBrowser
    {

        [Test]
        public void TestNavigation()
        {
            using (var browser = new Browser("http://www.tizag.com/htmlT/htmlselect.php"))
            {
                browser.Click(HtmlIdentifierType.Name, "sa");
                AssertFactory.Instance.ExpectUrl(browser, "https://cse.google.com/cse", startsWith: true);
            }
        }

    }
}
