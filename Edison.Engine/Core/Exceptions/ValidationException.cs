/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Engine.Core.Exceptions
{
    public class ValidationException : Exception
    {

        public ValidationException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

    }
}
