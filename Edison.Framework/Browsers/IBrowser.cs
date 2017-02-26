/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Framework
{
    public interface IBrowser : IDisposable
    {
        bool Visible { get; set; }

        string URL { get; }

        string Body { get; }



        void Navigate(string url);

        void ModifyUrl(string find, string replace);

        void Type(HtmlIdentifierType identifierType, string identifier, string value);

        string Value(HtmlIdentifierType identifierType, string identifier, bool useInnerHtml = false);

        void Click(HtmlIdentifierType identifierType, string identifier);

        void Check(HtmlIdentifierType identifierType, string identifier, bool uncheck = false);

        dynamic Get(HtmlIdentifierType identifierType, string identifier);

        void Sleep(int milliseconds);

        void SleepWhileBusy(int timeout = 30, int sleepTimeInMillis = 1000);

    }
}
