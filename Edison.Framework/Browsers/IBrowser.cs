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

        void MoodifyUrl(string find, string replace);

        void Sleep(int milliseconds);

        void SetElementValue(HtmlElementIdentifierType identifierType, string identifier, string value);

        string GetElementValue(HtmlElementIdentifierType identifierType, string identifier, bool getInnerHtml = false);

        void ClickElement(HtmlElementIdentifierType identifierType, string identifier);

        void CheckElement(HtmlElementIdentifierType identifierType, string identifier, bool uncheck = false);

        void SleepWhileBusy(int timeout = 30, int sleepTimeInMillis = 1000);

        dynamic GetElement(HtmlElementIdentifierType identifierType, string identifier);

    }
}
