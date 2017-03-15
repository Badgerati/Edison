/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using CommandLine;
using CommandLine.Text;
using Edison.Engine;
using Edison.Engine.Utilities.Extensions;
using Edison.Engine.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using YamlDotNet.Serialization;

namespace Edison.Console
{
    public class ConsoleOptions
    {

        #region Constants

        public const string EDISONFILE = "Edisonfile";

        public const string YamlAssemblies = "assemblies";

        public const string YamlOutputType = "output_type";
        public const string YamlConsoleOutputType = "console_output_type";

        public const string YamlDisableConsoleOutput = "disable_console_output";
        public const string YamlDisableFileOutput = "disable_file_output";
        public const string YamlDisableTestOutput = "disable_test_output";

        public const string YamlExclude = "exclude";
        public const string YamlInclude = "include";

        public const string YamlFixtures = "fixtures";
        public const string YamlTests = "tests";

        public const string YamlFixtureThreads = "fixture_threads";
        public const string YamlTestThreads = "test_threads";

        public const string YamlTestResultUrl = "test_result_url";
        public const string YamlTestRunId = "test_run_id";
        public const string YamlTestRunName = "test_run_name";
        public const string YamlTestRunProject = "test_run_project";
        public const string YamlTestRunEnv = "test_run_env";

        public const string YamlOutputDirectory = "output_directory";
        public const string YamlOutputFile = "output_file";

        public const string YamlRerun = "rerun";
        public const string YamlRerunThreshold = "rerun_threshold";

        public const string YamlSuite = "suite";

        public const string YamlSolution = "solution";
        public const string YamlSolutionConfig = "solution_config";

        public const string YamlSlackToken = "slack_token";

        #endregion

        #region Constructor

        public ConsoleOptions() { }

        public ConsoleOptions(Dictionary<object, object> yaml)
        {
            this.InjectPropertyDefaultValues();
            InjectYaml(yaml);
        }

        public ConsoleOptions InjectYaml(Dictionary<object, object> yaml)
        {
            this.InjectPropertyYamlValues(yaml);
            return this;
        }

        #endregion

        #region Options

        [YamlMember(Alias = YamlAssemblies)]
        [OptionList("a", Required = false, Separator = ',', HelpText = "List of paths to assemblies for testing.")]
        public IList<string> Assemblies { get; set; }


        [YamlMember(Alias = YamlDisableConsoleOutput)]
        [Option("dco", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether all output to the console is disabled.")]
        [DefaultValue(false)]
        public bool DisableConsoleOutput { get; set; }

        [YamlMember(Alias = YamlDisableFileOutput)]
        [Option("dfo", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether writing output to a file should be disabled.")]
        [DefaultValue(false)]
        public bool DisableFileOutput { get; set; }

        [YamlMember(Alias = YamlDisableTestOutput)]
        [Option("dto", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether user produced output from tests should be disabled.")]
        [DefaultValue(false)]
        public bool DisableTestOutput { get; set; }


        [YamlMember(Alias = YamlExclude)]
        [OptionList("e", Required = false, Separator = ',', HelpText = "List of categories that should be excluded.")]
        public IList<string> ExcludedCategories { get; set; }

        [YamlMember(Alias = YamlInclude)]
        [OptionList("i", Required = false, Separator = ',', HelpText = "List of categories that should be included.")]
        public IList<string> IncludedCategories { get; set; }


        [YamlMember(Alias = YamlOutputDirectory)]
        [Option("od", Required = false, HelpText = "Path to a directory where the output file produced should be stored.")]
        public string OutputDirectory { get; set; }

        [YamlMember(Alias = YamlOutputFile)]
        [Option("of", Required = false, DefaultValue = "ResultFile", HelpText = "Name of the output file created.")]
        [DefaultValue("ResultFile")]
        public string OutputFile { get; set; }


        [YamlMember(Alias = YamlOutputType)]
        [Option("ot", Required = false, DefaultValue = OutputType.Json, HelpText = "File output type format.")]
        [DefaultValue(OutputType.Json)]
        public OutputType OutputType { get; set; }

        [YamlMember(Alias = YamlConsoleOutputType)]
        [Option("cot", Required = false, DefaultValue = OutputType.Txt, HelpText = "Console output type format.")]
        [DefaultValue(OutputType.Txt)]
        public OutputType ConsoleOutputType { get; set; }


        [YamlMember(Alias = YamlTests)]
        [OptionList("t", Required = false, Separator = ',', HelpText = "List of Tests that should be run.")]
        public IList<string> Tests { get; set; }

        [YamlMember(Alias = YamlFixtures)]
        [OptionList("f", Required = false, Separator = ',', HelpText = "List of TestFixtures that should be run.")]
        public IList<string> Fixtures { get; set; }


        [YamlMember(Alias = YamlTestThreads)]
        [Option("tt", Required = false, DefaultValue = 1, HelpText = "Number of threads on which to execute the Tests within TestFixtures.")]
        [DefaultValue(1)]
        public int TestThreads { get; set; }

        [YamlMember(Alias = YamlFixtureThreads)]
        [Option("ft", Required = false, DefaultValue = 1, HelpText = "Number of threads on which to execute the TestFixtures.")]
        [DefaultValue(1)]
        public int FixtureThreads { get; set; }


        [YamlMember(Alias = YamlTestResultUrl)]
        [Option("turl", Required = false, HelpText = "Test result URL where test results and the other test run related data will be POSTed to after each test.")]
        public string TestResultUrl { get; set; }

        [YamlMember(Alias = YamlTestRunId)]
        [Option("tid", Required = false, HelpText = "Test run ID that can be used to identify this run, used with the test run URL.")]
        public string TestRunId { get; set; }

        [YamlMember(Alias = YamlTestRunName)]
        [Option("tname", Required = false, HelpText = "Test run's informative name, used in conjunction with TestRunId.")]
        public string TestRunName { get; set; }

        [YamlMember(Alias = YamlTestRunProject)]
        [Option("tproj", Required = false, HelpText = "Test run's project name, such as website, service, api, etc.")]
        public string TestRunProject { get; set; }

        [YamlMember(Alias = YamlTestRunEnv)]
        [Option("tenv", Required = false, HelpText = "Name of the environment this test run is occurring. Defaults the machine name.")]
        public string TestRunEnvironment { get; set; }


        [YamlMember(Alias = YamlRerun)]
        [Option("rft", Required = false, DefaultValue = false, HelpText = "Boolean flag specifying whether failed tests should be re-run post main threads. This re-run thread never run in parallel.")]
        [DefaultValue(false)]
        public bool RerunFailedTests { get; set; }

        [YamlMember(Alias = YamlRerunThreshold)]
        [Option("rt", Required = false, DefaultValue = 100, HelpText = "Value to specify the re-run failed test threshold. If the number of failed tests is greater than this percentage, the re-run will not happen.")]
        [DefaultValue(100)]
        public int RerunThreshold { get; set; }


        [YamlMember(Alias = YamlSuite)]
        [Option("s", Required = false, HelpText = "Name of the test Suite from which to run tests/fixtures.")]
        public string Suite { get; set; }


        [YamlMember(Alias = YamlSolution)]
        [Option("sln", Required = false, HelpText = "Path to a solution file to extract assemblies for testing.")]
        public string Solution { get; set; }

        [YamlMember(Alias = YamlSolutionConfig)]
        [Option("sconfig", Required = false, DefaultValue = "Debug", HelpText = "Solution's build configuration for locating assemblies.")]
        [DefaultValue("Debug")]
        public string SolutionConfiguration { get; set; }


        [YamlMember(Alias = YamlSlackToken)]
        [Option("slack", Required = false, HelpText = "Authorisation token to allow Edison to send messages to Slack channels")]
        public string SlackToken { get; set; }


        [YamlIgnore]
        [Option("ef", Required = false, HelpText = "Path to an Edisonfile")]
        public string Edisonfile { get; set; }

        #endregion

        #region Misc Options

        [YamlIgnore]
        [Option('h', "help", Required = false, HelpText = "Displays the help text.")]
        public bool ShowHelp { get; set; }

        [YamlIgnore]
        [Option('v', "version", Required = false, HelpText = "Displays the current version of Edison.")]
        public bool ShowVersion { get; set; }

        #endregion

        #region Help and Version

        public string GetVersion()
        {
            return Logger.Instance.GetVersion();
        }

        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        #endregion

    }
}
