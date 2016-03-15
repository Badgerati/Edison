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
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Edison.Engine.Validators
{
    public class NamespaceValidator : IValidator
    {

        #region Repositories

        private static IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        #endregion

        #region Validate

        public void Validate(EdisonContext context)
        {
            ValidateTests(context);
            ValidateFixtures(context);
        }

        #endregion

        #region Private Helpers

        private void ValidateTests(EdisonContext context)
        {
            var tests = context.Tests;
            if (tests == default(IList<string>) || !tests.Any())
            {
                return;
            }

            tests = tests.Distinct().ToList();
            var files = context.Tests.Where(x => x.Contains('\\') || x.Contains('/')).ToList();
            tests = tests.Where(x => !x.Contains('\\') && !x.Contains('/')).ToList();

            var _file = string.Empty;
            foreach (var file in files)
            {
                _file = file.Trim();

                if (!FileRepository.Exists(_file))
                {
                    throw new ValidationException("File for list of tests not found: '{0}'", _file);
                }

                tests.AddRange(FileRepository.ReadAllLines(_file, Encoding.UTF8));
            }

            context.Tests = tests.Distinct().ToList();
        }

        private void ValidateFixtures(EdisonContext context)
        {
            var fixtures = context.Fixtures;
            if (fixtures == default(IList<string>) || !fixtures.Any())
            {
                return;
            }

            fixtures = fixtures.Distinct().ToList();
            var files = fixtures.Where(x => x.Contains('\\') || x.Contains('/')).ToList();
            fixtures = fixtures.Where(x => !x.Contains('\\') && !x.Contains('/')).ToList();

            var _file = string.Empty;
            foreach (var file in files)
            {
               _file = file.Trim();

                if (!FileRepository.Exists(_file))
                {
                    throw new ValidationException("File for list of fixtures not found: '{0}'", _file);
                }

                fixtures.AddRange(FileRepository.ReadAllLines(_file, Encoding.UTF8));
            }

            context.Fixtures = fixtures.Distinct().ToList();
        }

        #endregion

    }
}
