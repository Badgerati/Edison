/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Core.Enums;
using Edison.Engine.Core.Output;
using Edison.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Engine
{
    public static class Logger
    {

        public static void WriteHelp()
        {
            Console.WriteLine(@"
            Edison :: Help Manual

            The following tags are accepted as input:

            -a      -   List of paths to assemblies (.dll) to run.
            -co     -   Boolean flag to state whether an output file should be created.
            -e      -   List of categories to be excluded.
            -f      -   List of TestFixtures that should be run.
            -help   -   Displays help manual (this page).
            -i      -   List of categories to be included.
            -id     -   Test run ID that can be used to identify this run.
            -od     -   Output directory for the output file created (default is working directory).
            -of     -   Name of the output file created.
            -ot     -   Type of the output file (csv, json, txt, xml).
            -t      -   Number of threads on which to execute the tests.
            -url    -   Test result URL where test results and ID will be POSTed to, after each test.
            -v      -   Version of Edison.
            ");
        }


        public static void WriteVersion()
        {
            Console.WriteLine("v0.1.1a");
        }


        public static void WriteError(string error)
        {
            Console.WriteLine(string.Format("{1}[ERROR]: {0}{1}", error, Environment.NewLine));
        }


        public static void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }


        public static void WriteInnerException(Exception ex, bool asMessage = false)
        {
            WriteException(ex.InnerException == default(Exception) ? ex : ex.InnerException, true);
        }


        public static void WriteException(Exception ex, bool asMessage = false)
        {
            if (asMessage)
            {
                WriteMessage(string.Format("{1}{0}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace));
            }
            else
            {
                WriteError(string.Format("{1}{0}{0}{2}", Environment.NewLine, ex.Message, ex.StackTrace));
            }
        }

        public static string CreateDirectory(string outputFolder, string directoryName, bool withDate = false)
        {
            if (withDate)
            {
                directoryName = string.Format("{0}_{1}", directoryName, GetDateString());
            }

            outputFolder = string.Format("{0}{1}{2}",
                outputFolder,
                outputFolder.EndsWith("/") || outputFolder.EndsWith("\\") ? string.Empty : "\\",
                directoryName);

            Directory.CreateDirectory(outputFolder);
            return outputFolder;
        }

        public static string GetDateString(bool inUtc = false)
        {
            var now = inUtc
                ? DateTime.UtcNow
                : DateTime.Now;

            return string.Format("{0:0000}-{1:00}-{2:00}_{3:00}-{4:00}-{5:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
        }

        public static string CreateFile(string outputFolder, string fileName, OutputType type, bool withDate = false)
        {
            var date = withDate
                ? GetDateString()
                : string.Empty;

            outputFolder = string.Format("{0}{1}{2}{3}.{4}",
                outputFolder,
                outputFolder.EndsWith("/") || outputFolder.EndsWith("\\") ? string.Empty : "\\",
                fileName,
                date,
                type.ToString().ToLower());

            var stream = File.Create(outputFolder);
            stream.Dispose();
            return outputFolder;
        }

        public static void WriteToFile(string filePath, string value)
        {
            File.AppendAllText(filePath, value);
        }

        public static void WriteResultToFile(string filePath, bool lastResult, TestResult result, IOutputRepository output) // OutputType type)
        {
            File.AppendAllText(filePath, output.ToString(result, !lastResult));
        }

        public static void WriteTestResult(TestResult result)
        {
            WriteMessage(OutputRepositoryManager.Get(OutputType.Txt).ToString(result, false));
        }

        public static void WriteSingleLine(string precede = "", string postcede = "")
        {
            WriteMessage(precede + "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -" + postcede);
        }

        public static void WriteDoubleLine(string precede = "", string postcede = "")
        {
            WriteMessage(precede + "= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =" + postcede);
        }

    }
}
