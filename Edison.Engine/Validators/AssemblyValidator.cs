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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Edison.Engine.Validators
{
    public class AssemblyValidator : IValidator
    {

        #region Constants

        public const string DllExtension = ".dll";

        public const string SlnExtension = ".sln";
        public const string SlnConfigDefault = "Debug";
        public const string SlnRegex = "Project\\(\\\"{.*?}\\\"\\) = \\\".*?\\\", \\\"(?<path>(.*?\\\\)*)(?<project>.*?).csproj\\\", \\\"{.*?}\\\"";

        #endregion

        #region Repositories

        private static IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        private static IPathRepository PathRepository
        {
            get { return DIContainer.Instance.Get<IPathRepository>(); }
        }

        #endregion

        #region Validate

        public void Validate(EdisonContext context)
        {
            // need to validate a solution first, for its assemblies
            SolutionValidator(context);

            // assemblies are mandatory, if there are none and no solution is passed then validation fails
            var assemblies = context.Assemblies;
            if (assemblies == default(IList<string>) || !assemblies.Any())
            {
                if (string.IsNullOrWhiteSpace(context.Solution))
                {
                    throw new ValidationException("No assembly or solution paths were supplied.");
                }

                return;
            }

            assemblies = assemblies.Distinct().ToList();
            var files = assemblies.Where(x => string.IsNullOrWhiteSpace(PathRepository.GetExtension(x.Trim()))).ToList();
            assemblies = assemblies.Where(x => !string.IsNullOrWhiteSpace(PathRepository.GetExtension(x.Trim()))).ToList();

            var _file = string.Empty;
            foreach (var file in files)
            {
                _file = file.Trim();

                if (!FileRepository.Exists(_file))
                {
                    throw new ValidationException("File for list of assemblies not found: '{0}'", _file);
                }

                assemblies.AddRange(FileRepository.ReadAllLines(_file, Encoding.UTF8));
            }

            assemblies = assemblies.Distinct().ToList();

            for (var i = 0; i < assemblies.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(assemblies[i]))
                {
                    throw new ValidationException("Assembly cannot be null or empty");
                }

                assemblies[i] = assemblies[i].Trim();

                if (!PathRepository.GetExtension(assemblies[i]).Equals(DllExtension, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ValidationException("Assembly is not a valid {0} file: '{1}'", DllExtension, assemblies[i]);
                }

                if (!FileRepository.Exists(assemblies[i]))
                {
                    throw new ValidationException("Assembly not found: '{0}'", assemblies[i]);
                }
            }

            context.Assemblies = assemblies;
        }

        #endregion

        #region Private Helpers

        private void SolutionValidator(EdisonContext context)
        {
            // solution is optional, so if not passed skip validation
            if (string.IsNullOrWhiteSpace(context.Solution))
            {
                return;
            }

            // validate solution configuration first
            if (string.IsNullOrWhiteSpace(context.SolutionConfiguration))
            {
                context.SolutionConfiguration = SlnConfigDefault;
            }

            context.Solution = context.Solution.Trim();

            if (!PathRepository.GetExtension(context.Solution).Equals(SlnExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ValidationException("Solution is not a valid {0} file: '{1}'", SlnExtension, context.Solution);
            }

            if (!FileRepository.Exists(context.Solution))
            {
                throw new ValidationException("Solution file not found: '{0}'", context.Solution);
            }

            var contents = FileRepository.ReadAllText(context.Solution, Encoding.UTF8);

            var groups = Regex.Matches(contents, SlnRegex)
                .Cast<Match>()
                .Select(x => x.Groups)
                .ToList();

            var solutionDir = PathRepository.GetDirectoryName(context.Solution);

            foreach (var group in groups)
            {
                var path = PathRepository.Combine(solutionDir, group["path"].Value, "bin", context.SolutionConfiguration, group["project"].Value + DllExtension);
                if (!FileRepository.Exists(path))
                {
                    throw new ValidationException("Assembly file from solution cannot be found: '{0}'", path);
                }

                context.Assemblies.Add(path);
            }
        }

        #endregion

    }
}
