/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Edison.Engine.Repositories;
using Edison.Framework;
using System;
using System.IO;
using System.Reflection;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;

namespace Edison.Engine
{
    public class Logger
    {

        #region Repositories

        private IOutputRepository _outputRepository;
        public IOutputRepository OutputRepository
        {
            get { return _outputRepository; }
            set
            {
                _outputRepository = value;

                if (_outputRepository != default(IOutputRepository) && _consoleOutputType != _outputRepository.OutputType)
                {
                    _consoleOutputType = _outputRepository.OutputType;
                }
            }
        }
        
        private IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        private IDirectoryRepository DirectoryRepository
        {
            get { return DIContainer.Instance.Get<IDirectoryRepository>(); }
        }

        private IDateTimeRepository DateTimeRepository
        {
            get { return DIContainer.Instance.Get<IDateTimeRepository>(); }
        }
        
        #endregion

        #region Properties

        public static Lazy<Logger> _lazy = new Lazy<Logger>(() => new Logger());
        public static Logger Instance
        {
            get { return _lazy.Value; }
        }
        
        public TextWriter OutStream { get; set; }
        public TextWriter ErrorStream { get; set; }

        private OutputType _consoleOutputType = OutputType.Txt;
        public OutputType ConsoleOutputType
        {
            get { return _consoleOutputType; }
            set
            {
                _consoleOutputType = value;

                if (OutputRepository == default(IOutputRepository) || _consoleOutputType != OutputRepository.OutputType)
                {
                    OutputRepository = OutputRepositoryFactory.Get(_consoleOutputType);
                }

                if (IsSingleOrNoLined)
                {
                    DisableConsole();
                }
            }
        }
        
        public bool IsSingleOrNoLined
        {
            get { return ConsoleOutputType == OutputType.None || ConsoleOutputType == OutputType.Dot; }
        }

        #endregion

        #region Constructor

        private Logger()
        {
            SetOutput(Console.Out, Console.Error);
            OutputRepository = OutputRepositoryFactory.Get(OutputType.Txt);
        }

        #endregion

        #region Public Helpers

        public void Disable()
        {
            SetOutput(TextWriter.Null);
        }

        public void DisableConsole()
        {
            SetConsoleOutput(TextWriter.Null);
        }

        public void SetOutput(TextWriter output)
        {
            SetOutput(output, output);
        }

        public void SetOutput(TextWriter output, TextWriter error)
        {
            OutStream = output;
            ErrorStream = error;
        }

        public void SetConsoleOutput(TextWriter output)
        {
            SetConsoleOutput(output, output);
        }

        public void SetConsoleOutput(TextWriter output, TextWriter error)
        {
            Console.SetOut(output);
            Console.SetError(error);
        }

        public void WriteHelp()
        {
            SetOutput(Console.Out, Console.Error);

            Console.WriteLine(@"
            Edison :: Help Manual
            
            The following tags are accepted as input:
            
            --a      -   List of paths to assemblies (.dll) to run, separated by spaces.

            --cot    -   Type of output for the console, default is TXT.

            --dco    -   Boolean flag to state whether all output to the console is
                        disabled (default is false).

            --dfo    -   Boolean flag to state whether an output file should be
                        disabled (default is false).

            --dto    -   Boolean flag to state whether user produced output from
                        tests should be disabled (default is false).

            --e      -   List of categories to be excluded, separated by spaces.

            --f      -   List of TestFixtures that should be run, separated by spaces.

            --help   -   Displays help manual (this page).

            --i      -   List of categories to be included, separated by spaces.

            --tid    -   Test run ID that can be used to identify this run.

            --od     -   Output directory for the output file created (default is
                        the working directory).

            --of     -   Name of the output file created.

            --ot     -   Type of the output file (default is JSON).

            --t      -   Number of threads on which to execute the tests (default is 1).

            --ts     -   List of Tests that should be run, separated by spaces.

            --url    -   Test result URL where test results and ID will be POSTed to,
                        after each test. Format is determined by -ot.

            --v      -   Displays the current version of Edison.



            The following are possible output types:
            
            XML     -   Typical XML formatted output.
            JSON    -   Typical JSON formatted output.
            TXT     -   Plain text output.
            CSV     -   Comma-separated output.
            Dot     -   Dot notation output. (ie, F=Fail, S=Skipped/Ignored, .=Success, I=Inconclusive)
            None    -   No output.
            ");
        }
        
        public void WriteVersion()
        {
            SetOutput(Console.Out, Console.Error);
            OutStream.WriteLine(GetVersion());
        }

        public string GetVersion()
        {
            return Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        public void WriteError(string error)
        {
            OutStream.WriteLine(string.Format("{1}[ERROR]: {0}{1}", error, Environment.NewLine));
        }

        public void WriteMessage(string message, bool singleLine = false)
        {
            if (singleLine)
            {
                OutStream.Write(message);
            }
            else
            {
                OutStream.WriteLine(message);
            }
        }

        public void WriteInnerException(Exception ex, bool asMessage = false)
        {
            WriteException(ex.InnerException == default(Exception) ? ex : ex.InnerException, true);
        }

        public void WriteException(Exception ex, bool asMessage = false)
        {
            if (asMessage)
            {
                if (IsSingleOrNoLined)
                {
                    return;
                }

                WriteMessage(string.Format("{1}{0}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace));
            }
            else
            {
                WriteError(string.Format("{1}{0}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace));
            }
        }

        public string CreateDirectory(string outputFolder, string directoryName, bool withDate = false)
        {
            if (withDate)
            {
                directoryName = string.Format("{0}_{1}", directoryName, GetDateString());
            }

            outputFolder = string.Format("{0}{1}{2}",
                outputFolder,
                outputFolder.EndsWith("/") || outputFolder.EndsWith("\\") ? string.Empty : "\\",
                directoryName);

            DirectoryRepository.CreateDirectory(outputFolder);
            return outputFolder;
        }

        public string GetDateString(bool inUtc = false)
        {
            var now = inUtc
                ? DateTimeRepository.UtcNow
                : DateTimeRepository.Now;

            return string.Format("{0:0000}-{1:00}-{2:00}_{3:00}-{4:00}-{5:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        }

        public string CreateFile(string outputFolder, string fileName, OutputType type, bool withDate = false)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            var date = withDate
                ? GetDateString()
                : string.Empty;

            outputFolder = string.Format("{0}{1}{2}{3}.{4}",
                outputFolder,
                outputFolder.EndsWith("/") || outputFolder.EndsWith("\\") ? string.Empty : "\\",
                fileName,
                date,
                type.ToString().ToLower());

            var stream = FileRepository.Create(outputFolder);
            stream.Dispose();
            return outputFolder;
        }

        public void WriteToFile(string filePath, string value)
        {
            FileRepository.AppendAllText(filePath, value);
        }

        public void WriteResultToFile(string filePath, bool lastResult, TestResult result, IOutputRepository output)
        {
            FileRepository.AppendAllText(filePath, output.ToString(result, !lastResult));
        }

        public void WriteTestResult(TestResult result)
        {
            if (ConsoleOutputType == OutputType.None)
            {
                return;
            }
            
            WriteMessage(OutputRepository.ToString(result, false), IsSingleOrNoLined);
        }

        public void WriteSingleLine(string precede = "", string postcede = "")
        {
            if (IsSingleOrNoLined)
            {
                return;
            }

            WriteMessage(precede + "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" + postcede);
        }

        public void WriteDoubleLine(string precede = "", string postcede = "")
        {
            if (IsSingleOrNoLined)
            {
                return;
            }

            WriteMessage(precede + "= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" + postcede);
        }

        #endregion

    }
}
