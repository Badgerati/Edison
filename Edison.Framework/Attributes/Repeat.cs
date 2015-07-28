/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RepeatAttribute : Attribute
    {
        public int Value = 1;

        public RepeatAttribute(int value = 1)
        {
            if (value <= 0)
            {
                throw new ArgumentException("Repeat value must be greater than 0.");
            }

            Value = value;
        }
    }
}
