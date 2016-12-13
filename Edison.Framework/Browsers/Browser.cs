/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using SHDocVw;
using System.Threading;
using System.Collections;
using System.Text.RegularExpressions;

namespace Edison.Framework
{
    public class Browser : IBrowser
    {

        #region Properties
        
        public bool Visible
        {
            get { return Explorer.Visible; }
            set { Explorer.Visible = value; }
        }

        public string URL
        {
            get { return Explorer.LocationURL; }
        }

        public string Body
        {
            get { return Explorer.Document.body.outerHTML; }
        }

        #endregion

        #region Fields

        private InternetExplorer Explorer = null;

        #endregion

        #region Constructor

        public Browser(bool visible = false)
        {
            Explorer = new InternetExplorer()
            {
                Visible = visible,
                Silent = true,
                TheaterMode = false
            };
        }

        #endregion

        #region Public Methods

        public void Dispose()
        {
            if (Explorer != default(InternetExplorer))
            {
                Explorer.Quit();
            }
        }

        public void Navigate(string url)
        {
            BrowserHelper.ValidateUrl(url);
            Explorer.Navigate(url);
            SleepWhileBusy();
        }

        public void MoodifyUrl(string find, string replace)
        {
            Navigate(URL.Replace(find, replace));
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public void SetElementValue(HtmlElementIdentifierType identifierType, string identifier, string value)
        {
            var control = GetElement(identifierType, identifier);
            
            if (control.Length > 1 && ((string)control[0].tagName).Equals("option", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var option in control)
                {
                    if (((string)option.innerHTML).Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        option.Selected = true;
                        break;
                    }
                }
            }
            else
            {
                control.value = value;
            }
        }

        public string GetElementValue(HtmlElementIdentifierType identifierType, string identifier, bool getInnerHtml = false)
        {
            var control = GetElement(identifierType, identifier);

            if (control.Length > 1 && ((string)control[0].tagName).Equals("option", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach (var option in control)
                {
                    if ((bool)option.Selected)
                    {
                        return option.innerHTML;
                    }
                }

                return string.Empty;
            }
            else
            {
                return getInnerHtml
                    ? control.innerHTML
                    : control.value;
            }
        }

        public void ClickElement(HtmlElementIdentifierType identifierType, string identifier)
        {
            var control = GetElement(identifierType, identifier);
            control.click();
            SleepWhileBusy();
        }

        public void CheckElement(HtmlElementIdentifierType identifierType, string identifier, bool uncheck = false)
        {
            var control = GetElement(identifierType, identifier);
            control.Checked = !uncheck;
            SleepWhileBusy();
        }

        public void SleepWhileBusy(int timeout = 30, int sleepTimeInMillis = 1000)
        {
            var count = 0;

            while (Explorer.Busy)
            {
                if (count >= timeout)
                {
                    throw new TimeoutException(string.Format("Loading URL has timed-out after {0} seconds", timeout));
                }

                Thread.Sleep(sleepTimeInMillis);
                count++;
            }
        }

        public dynamic GetElement(HtmlElementIdentifierType identifierType, string identifier)
        {
            if (Explorer.Document == null)
            {
                throw new NullReferenceException("Browser object has not Document available for querying");
            }

            var controls = default(dynamic);
            var control = default(dynamic);

            switch (identifierType)
            {
                case HtmlElementIdentifierType.ID:
                    control = Explorer.Document.getElementById(identifier);
                    break;

                case HtmlElementIdentifierType.Name:
                    controls = (IEnumerable)Explorer.Document.getElementsByName(identifier);
                    break;

                case HtmlElementIdentifierType.Tag:
                    controls = (IEnumerable)Explorer.Document.getElementsByTagName(identifier);
                    break;

                case HtmlElementIdentifierType.Class:
                    controls = (IEnumerable)Explorer.Document.getElementsByClassName(identifier);
                    break;

                default:
                    throw new ArgumentException(string.Format("Unrecognised HtmlElementIdentifierType: '{0}'", identifierType));
            }

            if (controls != null && controls.Length > 0)
            {
                control = controls[0];
            }

            if (control == null)
            {
                throw new ArgumentException(string.Format("No element with {0} of {1} found", identifierType, identifier));
            }

            return control;
        }

        #endregion

    }
}
