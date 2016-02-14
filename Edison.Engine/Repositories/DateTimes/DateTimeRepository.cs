﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Repositories.Interfaces;
using System;

namespace Edison.Engine.Repositories.DateTimes
{
    public class DateTimeRepository : IDateTimeRepository
    {

        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime UtcNow
        {
            get
            {
                return DateTime.UtcNow;
            }
        }

    }
}
