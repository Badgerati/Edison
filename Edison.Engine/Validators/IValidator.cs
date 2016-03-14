/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;

namespace Edison.Engine.Validators
{
    public interface IValidator
    {

        void Validate(EdisonContext context);

    }
}
