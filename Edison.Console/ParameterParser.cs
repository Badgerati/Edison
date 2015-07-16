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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Edison.Console
{
    public static class ParameterParser
    {

        #region Keywords

        private static IDictionary<string, Delegate> Keywords = new Dictionary<string, Delegate>
            {
                { "a", new Action<string[]>(AssemblyAction) },
                { "co", new Action<string[]>(CreateOutputAction) },
                { "e", new Action<string[]>(ExcludedAction) },
                { "f", new Action<string[]>(FixtureAction) },
                { "h", new Action<string[]>(HelpAction) },
                { "help", new Action<string[]>(HelpAction) },
                { "i", new Action<string[]>(IncludedAction) },
                { "id", new Action<string[]>(TestRunIdAction) },
                { "od", new Action<string[]>(OutputDirectoryAction) },
                { "of", new Action<string[]>(OutputFileAction) },
                { "ot", new Action<string[]>(OutputTypeAction) },
                { "t", new Action<string[]>(ThreadsAction) },
                { "url", new Action<string[]>(TestResultUrlAction) },
                { "version", new Action<string[]>(VersionAction) },
                { "v", new Action<string[]>(VersionAction) }
            };

        private static EdisonContext Context { get; set; }

        #endregion

        #region Parser

        public static bool Parse(EdisonContext context, string[] args)
        {
            if (args == default(string[]) || args.Length == 0)
            {
                HelpAction(default(string[]));
                return false;
            }

            Context = context;
            var keys = Keywords.Keys.ToList();

            var regex = new Regex("-(?<key>.+)");
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
                    Logger.WriteError(string.Format("Invalid argument keyword '{0}'.", key));
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

            foreach (var value in values)
            {
                if (!value.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ParseException(string.Format("Assembly it not a value dll: '{0}'", value));
                }

                if (!File.Exists(value))
                {
                    throw new ParseException(string.Format("Assembly not found: '{0}'", value));
                }
            }

            Context.AssemblyPaths.AddRange(values);
        }

        private static void HelpAction(string[] values)
        {
            Logger.WriteHelp();
        }

        private static void VersionAction(string[] values)
        {
            Logger.WriteVersion();
        }

        private static void ThreadsAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for -t. Expected 1 but got {0}", values.Length));
            }

            var threads = 1;
            if (!int.TryParse(values[0], out threads))
            {
                throw new ParseException(string.Format("Invalid integer supplied for -t: '{0}'", values[0]));
            }

            if (threads <= 0)
            {
                throw new ParseException(string.Format("Value must be greater than 0 for -t, but got '{0}'", values[0]));
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

            Context.Fixtures.AddRange(values);
        }

        private static void OutputFileAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for -of. Expected 1 but got {0}", values.Length));
            }

            Context.OutputFile = values[0];
        }

        private static void OutputDirectoryAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for -od. Expected 1 but got {0}", values.Length));
            }

            if (!Directory.Exists(values[0]))
            {
                throw new ParseException(string.Format("Directory supplied for -od does not exist: '{0}'", values[0]));
            }

            Context.OutputFolder = values[0];
        }

        private static void OutputTypeAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for -ot. Expected 1 but got {0}", values.Length));
            }

            var type = OutputType.Xml;
            if (!Enum.TryParse<OutputType>(values[0], true, out type))
            {
                throw new ParseException(string.Format("Output type supplied for -ot is incorrect: '{0}'", values[0]));
            }

            Context.OutputType = type;
        }

        private static void CreateOutputAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for -co. Expected 1 but got {0}", values.Length));
            }

            var create = true;
            if (!bool.TryParse(values[0], out create))
            {
                throw new ParseException(string.Format("Create output value supplied for -co is incorrect: '{0}'", values[0]));
            }

            Context.CreateOutput = create;
        }

        private static void TestResultUrlAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for -url. Expected 1 but got {0}", values.Length));
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
                throw new ParseException(string.Format("Connection to provided -url failed:\n{0}", ex.Message));
            }

            Context.TestResultURL = values[0];
        }

        private static void TestRunIdAction(string[] values)
        {
            if (values.Length != 1)
            {
                throw new ParseException(string.Format("Incorrect number of arguments supplied for -id. Expected 1 but got {0}", values.Length));
            }

            Context.TestRunId = values[0];
        }

        #endregion

    }
}
