/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SlackAttribute : Attribute
    {

        #region Properies

        public readonly string Channel;

        private SlackTestResultType _slackTestResult = SlackTestResultType.Failure;
        public SlackTestResultType SlackTestResult
        {
            get { return _slackTestResult; }
            set { _slackTestResult = value; }
        }

        #endregion

        #region Constructor

        public SlackAttribute(string channel)
        {
            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("Channel name cannot be null or empty.");
            }

            Channel = channel;
        }

        #endregion
    }
}
