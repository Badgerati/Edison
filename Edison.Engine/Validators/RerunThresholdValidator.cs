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
    public class RerunThresholdValidator : IValidator
    {

        #region Validate

        public void Validate(EdisonContext context)
        {
            if (context.RerunThreshold < 0)
            {
                throw new ValidationException("Value must be greater than or equal to 0 for re-run threshold, but got '{0}'", context.RerunThreshold);
            }

            if (context.RerunThreshold > 100)
            {
                throw new ValidationException("Value must be less than or equal to 100 for re-run threshold, but got '{0}'", context.RerunThreshold);
            }
        }

        #endregion

    }
}
