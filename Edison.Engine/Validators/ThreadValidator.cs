/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Exceptions;

namespace Edison.Engine.Validators
{
    public class ThreadValidator : IValidator
    {

        #region Validate

        public void Validate(EdisonContext context)
        {
            if (context.NumberOfFixtureThreads <= 0)
            {
                throw new ValidationException("Number of fixture threads supplied must be greater than 0, but got '{0}'", context.NumberOfFixtureThreads);
            }

            if (context.NumberOfTestThreads <= 0)
            {
                throw new ValidationException("Number of test threads supplied must be greater than 0, but got '{0}'", context.NumberOfTestThreads);
            }
        }

        #endregion

    }
}
