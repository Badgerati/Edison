/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using CommandLine;
using CommandLine.Text;
using Edison.Engine;
using Edison.Engine.Core.Enums;
using System.Collections.Generic;

namespace Edison.Console
{
    public class ConsoleOptions
    {

        [OptionList("a", Required = false, Separator = ',', HelpText = "List of paths to assemblies for testing.")]
        public IList<string> Assemblies { get; set; }

        [Option("cot", Required = false, DefaultValue = OutputType.Txt, HelpText = "Console output type format.")]
        public OutputType ConsoleOutputType { get; set; }

        [Option("dco", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether all output to the console is disabled.")]
        public bool DisableConsoleOutput { get; set; }

        [Option("dfo", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether writing output to a file should be disabled.")]
        public bool DisableFileOutput { get; set; }
        
        [Option("dto", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether user produced output from tests should be disabled.")]
        public bool DisableTestOutput { get; set; }

        [OptionList("e", Required = false, Separator = ',', HelpText = "List of categories that should be excluded.")]
        public IList<string> Excludes { get; set; }

        [OptionList("f", Required = false, Separator = ',', HelpText = "List of TestFixtures that should be run.")]
        public IList<string> Fixtures { get; set; }

        [Option("ft", Required = false, DefaultValue = 1, HelpText = "Number of threads on which to execute the TestFixtures.")]
        public int FixtureThreads { get; set; }

        [OptionList("i", Required = false, Separator = ',', HelpText = "List of categories that should be included.")]
        public IList<string> Includes { get; set; }

        [Option("tid", Required = false, HelpText = "Test run ID that can be used to identify this run, used with the test run URL.")]
        public string TestRunId { get; set; }

        [Option("od", Required = false, HelpText = "Path to a directory where the output file produced should be stored.")]
        public string OutputDirectory { get; set; }
        
        [Option("of", Required = false, DefaultValue = "ResultFile", HelpText = "Name of the output file created.")]
        public string OutputFile { get; set; }

        [Option("ot", Required = false, DefaultValue = OutputType.Json, HelpText = "Output file output type format.")]
        public OutputType OutputType { get; set; }

        [OptionList("t", Required = false, Separator = ',', HelpText = "List of Tests that should be run.")]
        public IList<string> Tests { get; set; }

        [Option("tt", Required = false, DefaultValue = 1, HelpText = "Number of threads on which to execute the Tests within TestFixtures.")]
        public int TestThreads { get; set; }

        [Option("url", Required = false, HelpText = "Test result URL where test results and the Test run ID will be POSTed to after each test.")]
        public string TestResultUrl { get; set; }

        [Option("rft", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether failed tests should be re-run post main threads. This re-run thread never run in parallel.")]
        public bool RerunFailedTests { get; set; }

        [Option("rt", Required = false, DefaultValue = 100, HelpText = "Value to specify the re-run failed test threshold. If the number of failed tests is greater than this percentage, the re-run will not happen.")]
        public int RerunThreshold { get; set; }

        [Option("s", Required = false, HelpText = "Name of the test Suite from which to run tests/fixtures.")]
        public string Suite { get; set; }

        [Option("sln", Required = false, HelpText = "Path to a solution file to extract assemblies for testing.")]
        public string Solution { get; set; }

        [Option("sconfig", Required = false, DefaultValue = "Debug", HelpText = "Solution's build configuration for locating assemblies.")]
        public string SolutionConfiguration { get; set; }



        [Option('h', "help", Required = false, HelpText = "Displays the help text.")]
        public bool ShowHelp { get; set; }

        [Option('v', "version", Required = false, HelpText = "Displays the current version of Edison.")]
        public bool ShowVersion { get; set; }

        
        public string GetVersion()
        {
            return Logger.Instance.GetVersion();
        }

        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }


    }
}
