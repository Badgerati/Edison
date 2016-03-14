/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using CommandLine;
using Edison.Engine;
using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Utilities.Helpers;
using Edison.Injector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

            var options = new ConsoleOptions();
            if (args == default(string[]) || args.Length == 0)
            {
                Logger.Instance.WriteMessage(options.GetUsage());
                return false;
            }
            
            if (!Parser.Default.ParseArguments(args, options))
            {
                Logger.Instance.WriteMessage(options.GetUsage());
                return false;
            }

            if (options.ShowHelp)
            {
                Logger.Instance.WriteMessage(options.GetUsage());
                return false;
            }

            if (options.ShowVersion)
            {
                Logger.Instance.WriteMessage(options.GetVersion());
                return false;
            }

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
            context.TestRunId = options.TestRunId;
            context.RerunThreshold = options.RerunThreshold;
            context.OutputFile = options.OutputFile;
            context.OutputDirectory = options.OutputDirectory;
            context.NumberOfFixtureThreads = options.FixtureThreads;
            context.NumberOfTestThreads = options.TestThreads;
            context.TestResultURL = options.TestResultUrl;
            
            return true;
        }

        #endregion

    }
}
