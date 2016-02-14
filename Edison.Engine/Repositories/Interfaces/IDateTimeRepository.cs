/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;

namespace Edison.Engine.Repositories.Interfaces
{
    public interface IDateTimeRepository
    {

        DateTime Now { get; }
        DateTime UtcNow { get; }

    }
}
