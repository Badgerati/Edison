/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System;

namespace Edison.Engine.Validators
{
    public class OutputValidator : IValidator
    {

        #region Constants

        public const string OutputFileName = "ResultFile";

        #endregion

        #region Repositories

        private static IDirectoryRepository DirectoryRepository
        {
            get { return DIContainer.Instance.Get<IDirectoryRepository>(); }
        }

        #endregion

        #region Validate

        public void Validate(EdisonContext context)
        {
            if (string.IsNullOrWhiteSpace(context.OutputFile))
            {
                context.OutputFile = OutputFileName;
            }

            if (string.IsNullOrWhiteSpace(context.OutputDirectory))
            {
                context.OutputDirectory = Environment.CurrentDirectory;
            }

            if (!DirectoryRepository.Exists(context.OutputDirectory))
            {
                throw new ValidationException("Output folder supplied does not exist: '{0}'", context.OutputDirectory);
            }
        }

        #endregion

    }
}
