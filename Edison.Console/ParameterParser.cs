/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using CommandLine;
using Edison.Engine;
using Edison.Engine.Contexts;
using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Utilities.Helpers;
using Edison.Injector;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Edison.Console
{
    public static class ParameterParser
    {

        #region Repositories

        private static IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        private static IDirectoryRepository DirectoryRepository
        {
            get { return DIContainer.Instance.Get<IDirectoryRepository>(); }
        }

        private static IWebRequestRepository WebRequestRepository
        {
            get { return DIContainer.Instance.Get<IWebRequestRepository>(); }
        }

        private static IPathRepository PathRepository
        {
            get { return DIContainer.Instance.Get<IPathRepository>(); }
        }

        #endregion

        #region Parser

        public static bool Parse(EdisonContext context, string[] args)
        {
            if (context == default(EdisonContext))
            {
                throw new Exception("No EdisonContext supplied for parsing parameters");
            }

            // Check to see if any arguments were passed, or that an Edisonfile exists
            var options = new ConsoleOptions();
            var anyArgs = (args != default(string[]) && args.Length > 0);
            if (!anyArgs && !File.Exists(ConsoleOptions.EDISONFILE))
            {
                Logger.Instance.WriteMessage(options.GetUsage());
                return false;
            }

            // Attempt to parse then into a the options
            if (!Parser.Default.ParseArguments(args, options))
            {
                Logger.Instance.WriteMessage(options.GetUsage());
                return false;
            }

            // If there were no arguments passed, or the option's Edisonfile is set, serialize the options
            if (!anyArgs || !string.IsNullOrWhiteSpace(options.Edisonfile))
            {
                var file = string.IsNullOrWhiteSpace(options.Edisonfile) ? ConsoleOptions.EDISONFILE : options.Edisonfile;
                if (!File.Exists(file))
                {
                    throw new ArgumentException(string.Format("Edisonfile does not exist: {0}", file));
                }

                var yaml = new Deserializer(namingConvention: new UnderscoredNamingConvention());

                using (var reader = new StreamReader(file))
                {
                    options.InjectYaml(yaml.Deserialize<Dictionary<object, object>>(reader));
                }
            }

            // Show the help usage text if required
            if (options.ShowHelp)
            {
                Logger.Instance.WriteMessage(options.GetUsage());
                return false;
            }

            // Show the version of Edison if required
            if (options.ShowVersion)
            {
                Logger.Instance.WriteMessage(options.GetVersion());
                return false;
            }

            // assign all values from the console/edisonfile to the main context runner
            context.Assemblies = new List<string>(EnumerableHelper.SafeGuard(options.Assemblies));
            context.IncludedCategories = new List<string>(EnumerableHelper.SafeGuard(options.IncludedCategories));
            context.ExcludedCategories = new List<string>(EnumerableHelper.SafeGuard(options.ExcludedCategories));
            context.Tests = new List<string>(EnumerableHelper.SafeGuard(options.Tests));
            context.Fixtures = new List<string>(EnumerableHelper.SafeGuard(options.Fixtures));
            context.Solution = options.Solution;
            context.SolutionConfiguration = options.SolutionConfiguration;
            context.Suite = options.Suite;
            context.RerunFailedTests = options.RerunFailedTests;
            context.DisableFileOutput = options.DisableFileOutput;
            context.DisableConsoleOutput = options.DisableConsoleOutput;
            context.DisableTestOutput = options.DisableTestOutput;
            context.ConsoleOutputType = options.ConsoleOutputType;
            context.OutputType = options.OutputType;
            context.UrlOutputType = options.UrlOutputType;
            context.TestRunId = options.TestRunId;
            context.RerunThreshold = options.RerunThreshold;
            context.OutputFile = options.OutputFile;
            context.OutputDirectory = options.OutputDirectory;
            context.NumberOfFixtureThreads = options.FixtureThreads;
            context.NumberOfTestThreads = options.TestThreads;
            context.TestResultURL = options.TestResultUrl;
            context.SlackToken = options.SlackToken;

            return true;
        }

        #endregion

    }
}
