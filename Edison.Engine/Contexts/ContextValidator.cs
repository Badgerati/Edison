/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */
 
using Edison.Engine.Validators;
using System.Linq;
using System.Collections.Generic;

namespace Edison.Engine.Contexts
{
    public static class ContextValidator
    {

        /// <summary>
        /// Validates the specified context. Where properties don't match what the validator expects, then a
        /// ValidationException will be thrown. If a property is empty and not required, then the validator
        /// will default the value.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void Validate(EdisonContext context)
        {
            Validate(context,
                new AssemblyValidator(),
                new RerunThresholdValidator(),
                new OutputValidator(),
                new ThreadValidator(),
                new TestResultUrlValidator(),
                new NamespaceValidator());
        }

        /// <summary>
        /// Validates the specified context using the specified validators. Where properties don't match what
        /// the validator expects, then a ValidationException will be thrown. If a property is empty and not
        /// required, then the validator will default the value.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="validators">The validators.</param>
        public static void Validate(EdisonContext context, params IValidator[] validators)
        {
            validators.ToList().ForEach(x => x.Validate(context));
        }

    }
}
