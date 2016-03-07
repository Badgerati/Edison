/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RepeatAttribute : Attribute
    {
        public readonly int Value;
        public readonly bool Parallel;

        public RepeatAttribute(int value) : this(value, false)
        {
        }

        protected RepeatAttribute(int value, bool parallel)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Repeat value must be greater than 0.");
            }

            Value = value;
            Parallel = parallel;
        }
    }
}
