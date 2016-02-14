/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine;
using Edison.Engine.Contexts;
using Edison.Engine.Core.Enums;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Edison.Console
{
    public static class ParameterParser
    {

        #region Keywords

        private static IDictionary<string, Delegate> Keywords = new Dictionary<string, Delegate>
            {
                { "a", new Action<string[]>(AssemblyAction) },
                { "dfo", new Action<string[]>(DisableFileOutputAction) },
                { "e", new Action<string[]>(ExcludedAction) },
                { "f", new Action<string[]>(FixtureAction) },
                { "h", new Action<string[]>(HelpAction) },
                { "help", new Action<string[]>(HelpAction) },
                { "i", new Action<string[]>(IncludedAction) },
                { "tid", new Action<string[]>(TestRunIdAction) },
                { "od", new Action<string[]>(OutputDirectoryAction) },
                { "of", new Action<string[]>(OutputFileAction) },
                { "ot", new Action<string[]>(OutputTypeAction) },
                { "t", new Action<string[]>(ThreadsAction) },
                { "ts", new Action<string[]>(TestsAction) },
                { "url", new Action<string[]>(TestResultUrlAction) },
                { "version", new Action<string[]>(VersionAction) },
                { "v", new Action<string[]>(VersionAction) },
                { "dco", new Action<string[]>(DisableOutputAction) },
                { "cot", new Action<string[]>(ConsoleOutputTypeAction) },
                { "dto", new Action<string[]>(DisableTestOutputAction) }
            };

        private static EdisonContext Context;
        private static IFileRepository FileRepository;

        #endregion

        #region Parser

        public static bool Parse(EdisonContext context, string[] args, IFileRepository fileRepository)
        {
            if (context == default(EdisonContext))
            {
                throw new Exception("No EdisonContext supplied for parsing parameters");
            }

            if (fileRepository == default(IFileRepository))
            {
                throw new Exception("No FileRepository supplied for parsing parameters");
            }

            if (args == default(string[]) || args.Length == 0)
            {
                HelpAction(default(string[]));
                return false;
            }

            Context = context;
            FileRepository = fileRepository;

            var keys = Keywords.Keys.ToList();

            var regex = new Regex("--(?<key>.+)");
            var sets = new Dictionary<string, IList<string>>();
            var currentKey = string.Empty;

            foreach (var arg in args)
            {
                if (regex.IsMatch(arg))
                {
                    currentKey = regex.Match(arg).Groups["key"].Value;

                    if (!sets.ContainsKey(currentKey))
                    {
                        sets.Add(currentKey, new List<string>());
                    }
                }
                else if (!string.IsNullOrEmpty(currentKey))
                {
                    sets[currentKey].Add(arg);
                }
            }

            if (sets.Count == 0)
            {
                HelpAction(default(string[]));
                return false;
            }

            var passedKeys = sets.Keys.ToList();

            foreach (var key in passedKeys)
            {
                if (keys.Contains(key))
                {
                    if (key == "help" || key == "h")
                    {
                        HelpAction(default(string[]));
                        return false;
                    }

                    if (key == "v" || key == "version")
                    {
                        VersionAction(default(string[]));
                        return false;
                    }

                    Keywords[key].DynamicInvoke(new object[] { sets[key].ToArray() });
                }
                else
                {
                    Logger.Instance.WriteError(string.Format("Invalid argument keyword '{0}'.", key));
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Actions

        private static void AssemblyAction(string[] values)
        {
            if (values.Length == 0)
            {
                throw new ParseException("No assembly paths supplied");
            }

            const string extension = ".dll";
            var assemblies = new List<string>(values.Length);
            var files = values.Where(x => string.IsNullOrEmpty(Path.GetExtension(x.Trim())));
            values = values.Where(x => !string.IsNullOrEmpty(Path.GetExtension(x.Trim()))).ToArray();

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
                    if (Path.GetExtension(assembly) != extension)
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

                if (Path.GetExtension(_value) != extension)
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

        private static void HelpAction(string[] values)
        {
            Logger.Instance.WriteHelp();
        }

        private static void VersionAction(string[] values)
        {
            Logger.Instance.WriteVersion();
        }

        private static void ThreadsAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for number of threads. Expected 1 but got {0}", values.Length));
            }

            var threads = 1;
            if (!int.TryParse(values[0], out threads))
            {
                throw new ParseException(string.Format("Invalid integer supplied for number of threads: '{0}'", values[0]));
            }

            if (threads <= 0)
            {
                throw new ParseException(string.Format("Value must be greater than 0 for threading, but got '{0}'", values[0]));
            }

            Context.NumberOfThreads = threads;
        }

        private static void IncludedAction(string[] values)
        {
            if (values.Length == 0)
            {
                throw new ParseException("No included categories supplied");
            }

            if (Context.ExcludedCategories.Count > 0)
            {
                throw new ParseException("Cannot supply both included and excluded categories");
            }

            Context.IncludedCategories.AddRange(values);
        }

        private static void ExcludedAction(string[] values)
        {
            if (values.Length == 0)
            {
                throw new ParseException("No excluded categories supplied");
            }

            if (Context.IncludedCategories.Count > 0)
            {
                throw new ParseException("Cannot supply both included and excluded categories");
            }

            Context.ExcludedCategories.AddRange(values);
        }

        private static void FixtureAction(string[] values)
        {
            if (values.Length == 0)
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

        private static void TestsAction(string[] values)
        {
            if (values.Length == 0)
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

        private static void OutputFileAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied output file name. Expected 1 but got {0}", values.Length));
            }

            Context.OutputFile = values[0];
        }

        private static void OutputDirectoryAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for output directory. Expected 1 but got {0}", values.Length));
            }

            if (!Directory.Exists(values[0]))
            {
                throw new ParseException(string.Format("Output directory supplied does not exist: '{0}'", values[0]));
            }

            Context.OutputFolder = values[0];
        }

        private static void OutputTypeAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for output type Expected 1 but got {0}", values.Length));
            }

            var type = OutputType.Xml;
            if (!Enum.TryParse<OutputType>(values[0], true, out type))
            {
                throw new ParseException(string.Format("Output type supplied is incorrect: '{0}'", values[0]));
            }

            Context.OutputType = type;
        }

        private static void TestResultUrlAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for test run URL. Expected 1 but got {0}", values.Length));
            }

            try
            {
                var request = (WebRequest)HttpWebRequest.Create(values[0]);
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

            Context.TestResultURL = values[0];
        }

        private static void TestRunIdAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for test run ID. Expected 1 but got {0}", values.Length));
            }

            Context.TestRunId = values[0];
        }

        private static void ConsoleOutputTypeAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for console output type. Expected 1 but got {0}", values.Length));
            }

            var type = OutputType.Xml;
            if (!Enum.TryParse<OutputType>(values[0], true, out type))
            {
                throw new ParseException(string.Format("Console output type supplied is incorrect: '{0}'", values[0]));
            }

            Context.ConsoleOutputType = type;
        }

        private static void DisableOutputAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for disabling console output. Expected 1 but got {0}", values.Length));
            }

            var disbale = true;
            if (!bool.TryParse(values[0], out disbale))
            {
                throw new ParseException(string.Format("Disable console output value supplied is incorrect: '{0}'", values[0]));
            }

            Context.DisableConsoleOutput = disbale;
        }

        private static void DisableTestOutputAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for disabling test output. Expected 1 but got {0}", values.Length));
            }

            var disbale = true;
            if (!bool.TryParse(values[0], out disbale))
            {
                throw new ParseException(string.Format("Disable test output value supplied is incorrect: '{0}'", values[0]));
            }

            Context.DisableTestOutput = disbale;
        }

        private static void DisableFileOutputAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for disabling file output. Expected 1 but got {0}", values.Length));
            }

            var disable = true;
            if (!bool.TryParse(values[0], out disable))
            {
                throw new ParseException(string.Format("Disable file output value supplied is incorrect: '{0}'", values[0]));
            }

            Context.DisableFileOutput = disable;
        }

        #endregion

    }
}
