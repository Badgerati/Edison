/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System.Web;

namespace Edison.Engine.Utilities.Extensions
{
    public static class StringExtension
    {

        public static string ToHtmlString(this string src)
        {
            return HttpUtility.HtmlEncode(src);
        }

        public static string ToUrlString(this string src)
        {
            return HttpUtility.UrlEncode(src);
        }

        public static string Safeguard(this string src)
        {
            return string.IsNullOrEmpty(src)
                ? string.Empty
                : src;
        }

    }
}
