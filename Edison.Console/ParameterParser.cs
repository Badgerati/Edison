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
using Edison.Injector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        
        #region Instance Variables
        
        private static EdisonContext Context;

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
                options.GetUsage();
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

            Context = context;

            AssemblyAction(options.Assemblies);
            FixtureThreadsAction(options.FixtureThreads);
            TestThreadsAction(options.TestThreads);
            IncludedAction(options.Includes);
            ExcludedAction(options.Excludes);
            FixtureAction(options.Fixtures);
            TestsAction(options.Tests);
            OutputFileAction(options.OutputFile);
            OutputDirectoryAction(options.OutputDirectory);
            OutputTypeAction(options.OutputType);
            TestResultUrlAction(options.TestResultUrl);
            TestRunIdAction(options.TestRunId);
            ConsoleOutputTypeAction(options.ConsoleOutputType);
            DisableOutputAction(options.DisableConsoleOutput);
            DisableTestOutputAction(options.DisableTestOutput);
            DisableFileOutputAction(options.DisableFileOutput);

            return true;
        }

        #endregion

        #region Actions

        private static void AssemblyAction(IList<string> values)
        {
            if (values == default(IList<string>) || values.Count == 0)
            {
                throw new ParseException("No assembly paths supplied");
            }

            const string extension = ".dll";
            var assemblies = new List<string>(values.Count);
            var files = values.Where(x => string.IsNullOrEmpty(PathRepository.GetExtension(x.Trim())));
            values = values.Where(x => !string.IsNullOrEmpty(PathRepository.GetExtension(x.Trim()))).ToArray();

            foreach (var file in files)
            {
                var _file = file.Trim();

                if (!FileRepository.Exists(_file))
                {
                    throw new ParseException(string.Format("File for list of asemblies not found: '{0}'", _file));
                }

                var possibleAssemblies = FileRepository.ReadAllLines(_file);

                foreach (var assembly in possibleAssemblies)
                {
                    if (PathRepository.GetExtension(assembly) != extension)
                    {
                        throw new ParseException(string.Format("Assembly it not a valid dll: '{0}' in file '{1}'", assembly, _file));
                    }

                    if (!FileRepository.Exists(assembly))
                    {
                        throw new ParseException(string.Format("Assembly not found: '{0}' in file '{1}'", assembly, _file));
                    }
                }

                assemblies.AddRange(possibleAssemblies);
            }

            foreach (var value in values)
            {
                var _value = value.Trim();

                if (PathRepository.GetExtension(_value) != extension)
                {
                    throw new ParseException(string.Format("Assembly it not a valid dll: '{0}'", _value));
                }

                if (!FileRepository.Exists(_value))
                {
                    throw new ParseException(string.Format("Assembly not found: '{0}'", _value));
                }

                assemblies.Add(_value);
            }

            Context.AssemblyPaths.AddRange(assemblies);
        }

        private static void FixtureThreadsAction(int value)
        {
            if (value <= 0)
            {
                throw new ParseException(string.Format("Value must be greater than 0 for fixture threading, but got '{0}'", value));
            }

            Context.NumberOfFixtureThreads = value;
        }

        private static void TestThreadsAction(int value)
        {
            if (value <= 0)
            {
                throw new ParseException(string.Format("Value must be greater than 0 for test threading, but got '{0}'", value));
            }

            Context.NumberOfTestThreads = value;
        }

        private static void IncludedAction(IList<string> values)
        {
            if (values == default(IList<string>))
            {
                return;
            }

            if (values.Count == 0)
            {
                throw new ParseException("No included categories supplied");
            }

            Context.IncludedCategories.AddRange(values);
        }

        private static void ExcludedAction(IList<string> values)
        {
            if (values == default(IList<string>))
            {
                return;
            }

            if (values.Count == 0)
            {
                throw new ParseException("No excluded categories supplied");
            }

            Context.ExcludedCategories.AddRange(values);
        }

        private static void FixtureAction(IList<string> values)
        {
            if (values == default(IList<string>))
            {
                return;
            }

            if (values.Count == 0)
            {
                throw new ParseException("No fixtures supplied");
            }
            
            var files = values.Where(x => x.Contains('\\') || x.Contains('/'));
            var fixtures = values.Where(x => !x.Contains('\\') && !x.Contains('/')).ToList();

            foreach (var file in files)
            {
                var _file = file.Trim();

                if (!FileRepository.Exists(_file))
                {
                    throw new ParseException(string.Format("File for list of fixtures not found: '{0}'", _file));
                }

                fixtures.AddRange(FileRepository.ReadAllLines(_file));
            }

            Context.Fixtures.AddRange(fixtures.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static void TestsAction(IList<string> values)
        {
            if (values == default(IList<string>))
            {
                return;
            }

            if (values.Count == 0)
            {
                throw new ParseException("No tests supplied");
            }
            
            var files = values.Where(x => x.Contains('\\') || x.Contains('/'));
            var tests = values.Where(x => !x.Contains('\\') && !x.Contains('/')).ToList();

            foreach (var file in files)
            {
                var _file = file.Trim();

                if (!FileRepository.Exists(_file))
                {
                    throw new ParseException(string.Format("File for list of tests not found: '{0}'", _file));
                }

                tests.AddRange(FileRepository.ReadAllLines(_file));
            }

            Context.Tests.AddRange(tests.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        private static void OutputFileAction(string value)
        {
            Context.OutputFile = value;
        }

        private static void OutputDirectoryAction(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (!DirectoryRepository.Exists(value))
            {
                throw new ParseException(string.Format("Output directory supplied does not exist: '{0}'", value));
            }

            Context.OutputFolder = value;
        }

        private static void OutputTypeAction(OutputType value)
        {
            Context.OutputType = value;
        }

        private static void TestResultUrlAction(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            try
            {
                var request = WebRequestRepository.Create(value);
                request.Timeout = 30;

                using (var response = request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        reader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ParseException(string.Format("Connection to provided test run URL failed:\n{0}", ex.Message));
            }

            Context.TestResultURL = value;
        }

        private static void TestRunIdAction(string value)
        {
            Context.TestRunId = value;
        }

        private static void ConsoleOutputTypeAction(OutputType value)
        {
            Context.ConsoleOutputType = value;
        }

        private static void DisableOutputAction(bool value)
        {
            Context.DisableConsoleOutput = value;
        }

        private static void DisableTestOutputAction(bool value)
        {
            Context.DisableTestOutput = value;
        }

        private static void DisableFileOutputAction(bool value)
        {
            Context.DisableFileOutput = value;
        }

        #endregion

    }
}
