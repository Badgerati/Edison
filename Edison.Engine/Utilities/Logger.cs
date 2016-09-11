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
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.Text;
using Edison.Engine.Repositories.Outputs;

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

        private IAssemblyRepository AssemblyRepository
        {
            get { return DIContainer.Instance.Get<IAssemblyRepository>(); }
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

        public void WriteVersion()
        {
            SetOutput(Console.Out, Console.Error);
            OutStream.WriteLine(GetVersion());
        }

        public string GetVersion()
        {
            return AssemblyRepository.GetEntryAssembly().GetName().Version.ToString();
        }

        public void WriteError(string error)
        {
            ErrorStream.WriteLine(string.Format("{1}[ERROR]: {0}{1}", error, Environment.NewLine));
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

        public void WriteObject(object obj, bool singleLine = false)
        {
            if (singleLine)
            {
                OutStream.Write(obj.ToString());
            }
            else
            {
                OutStream.WriteLine(obj.ToString());
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
                OutputRepositoryFactory.Get(type).Extension);

            var stream = FileRepository.Create(outputFolder);
            stream.Dispose();
            return outputFolder;
        }

        public void WriteToFile(string filePath, string value)
        {
            FileRepository.AppendAllText(filePath, value, Encoding.UTF8);
        }

        public void WriteResultToFile(string filePath, bool lastResult, TestResult result, IOutputRepository output)
        {
            FileRepository.AppendAllText(filePath, output.ToString(result, !lastResult), Encoding.UTF8);
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
