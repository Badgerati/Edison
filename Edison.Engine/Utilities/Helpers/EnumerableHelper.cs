/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System.Collections.Generic;
using System.Linq;

namespace Edison.Engine.Utilities.Helpers
{
    public static class EnumerableHelper
    {

        public static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable)
        {
            return enumerable == default(IEnumerable<T>) || !enumerable.Any();
        }

    }
}
