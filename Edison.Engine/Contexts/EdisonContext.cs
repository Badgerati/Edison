/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Edison.Engine.Threading;
using Edison.Engine.Core.Enums;
using Edison.Engine.Utilities.Structures;
using System.Diagnostics;
using Edison.Engine.Repositories;
using Edison.Framework.Enums;
using Edison.Engine.Events;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.Threading.Tasks;
using Edison.Engine.Utilities.Helpers;
using System.Text.RegularExpressions;
using System.Text;

namespace Edison.Engine.Contexts
{
    public class EdisonContext
    {

        #region Repositories

        private IAssemblyRepository AssemblyRepository
        {
            get { return DIContainer.Instance.Get<IAssemblyRepository>(); }
        }

        private IPathRepository PathRepository
        {
            get { return DIContainer.Instance.Get<IPathRepository>(); }
        }

        private IReflectionRepository ReflectionRepository
        {
            get { return DIContainer.Instance.Get<IReflectionRepository>(); }
        }

        private IFileRepository FileRepository
        {
            get { return DIContainer.Instance.Get<IFileRepository>(); }
        }

        #endregion

        #region Properties

        public bool IsRunning { get; private set; }
        public string CurrentAssembly { get; private set; }

        public List<string> AssemblyPaths { get; private set; }
        public List<string> IncludedCategories { get; private set; }
        public List<string> ExcludedCategories { get; private set; }
        public List<string> Fixtures { get; private set; }
        public List<string> Tests { get; private set; }
        public int NumberOfFixtureThreads { get; set; }
        public int NumberOfTestThreads { get; set; }
        public string OutputFile { get; set; }
        public string OutputFolder { get; set; }
        public OutputType OutputType { get; set; }
        public OutputType ConsoleOutputType { get; set; }
        public bool DisableFileOutput { get; set; }
        public bool DisableConsoleOutput { get; set; }
        public bool DisableTestOutput { get; set; }
        public string TestResultURL { get; set; }
        public string TestRunId { get; set; }
        public bool RerunFailedTests { get; set; }
        public int RerunThreshold { get; set; }
        public string Suite { get; set; }
        public string Solution { get; set; }
        public string SolutionConfiguration { get; set; }

        private Stopwatch Timer = default(Stopwatch);
        private TestResultDictionary ResultQueue;

        private IList<TestFixtureThread> ParallelThreads = default(IList<TestFixtureThread>);
        private Task ParallelTask = default(Task);
        private TestFixtureThread SingularThread = default(TestFixtureThread);
        private Task SingularTask = default(Task);

        #endregion

        #region Events

        public event TestResultEventHandler OnTestResult;

        #endregion

        #region Constructor

        public EdisonContext()
        {
            IsRunning = false;
            AssemblyPaths = new List<string>(1);
            IncludedCategories = new List<string>();
            ExcludedCategories = new List<string>();
            Fixtures = new List<string>();
            Tests = new List<string>();
            NumberOfFixtureThreads = 1;
            NumberOfTestThreads = 1;
            ConsoleOutputType = OutputType.Txt;
            OutputType = OutputType.Json;
            OutputFolder = Environment.CurrentDirectory;
            OutputFile = "ResultFile";
            DisableFileOutput = false;
            DisableConsoleOutput = false;
            DisableTestOutput = false;
            TestResultURL = string.Empty;
            IsRunning = true;
            RerunFailedTests = false;
            RerunThreshold = 100;
        }

        #endregion

        #region Public Methods

        public TestResultDictionary Run()
        {
            //set logging output
            if (DisableConsoleOutput)
            {
                Logger.Instance.Disable();
                Logger.Instance.DisableConsole();
            }

            if (DisableTestOutput)
            {
                Logger.Instance.DisableConsole();
            }

            //set output logging type
            Logger.Instance.ConsoleOutputType = ConsoleOutputType;

            //start timer
            Timer = new Stopwatch();
            Timer.Start();

            //create queue
            ResultQueue = new TestResultDictionary(this);

            //bind test result events
            if (OnTestResult != default(TestResultEventHandler))
            {
                ResultQueue.OnTestResult += OnTestResult;
            }

            //bind assembly event resolver
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            //grab assemblies from solution file
            if (!string.IsNullOrWhiteSpace(Solution))
            {
                var contents = FileRepository.ReadAllText(Solution, Encoding.UTF8);

                var regex = new Regex("Project\\(\\\"{.*?}\\\"\\) = \\\".*?\\\", \\\"(?<path>(.*?\\\\)*)(?<project>.*?).csproj\\\", \\\"{.*?}\\\"");
                var groups = regex.Matches(contents)
                    .Cast<Match>()
                    .Select(x => x.Groups)
                    .ToList();

                var solutionDir = PathRepository.GetDirectoryName(Solution);

                foreach (var group in groups)
                {
                    var path = PathRepository.Combine(solutionDir, group["path"].Value, "bin", SolutionConfiguration, group["project"].Value + ".dll");
                    if (!FileRepository.Exists(path))
                    {
                        continue;
                    }

                    AssemblyPaths.Add(path);
                }
            }

            //loop through all assemblies, running their tests
            var assemblies = AssemblyPaths.Distinct();
            foreach (var assemblyPath in assemblies)
            {
                CurrentAssembly = PathRepository.GetFileName(assemblyPath);
                var assembly = AssemblyRepository.LoadFile(assemblyPath);

                //global setup
                var globalSetupFixture = AssemblyRepository.GetTypes<SetupFixtureAttribute>(assembly, default(IList<string>), default(IList<string>), null).SingleOrDefault();
                var globalActivator = default(object);
                var globalSetupEx = default(Exception);

                if (globalSetupFixture != default(Type))
                {
                    globalActivator = Activator.CreateInstance(globalSetupFixture, null);
                    globalSetupEx = RunGlobalSetup(globalSetupFixture, globalActivator);
                }

                //test fixtures an threads
                RunThreads(assembly, globalSetupEx);

                //global teardown
                if (globalSetupFixture != default(Type))
                {
                    RunGlobalTeardown(globalSetupFixture, globalActivator);
                }
            }

            Timer.Stop();

            //if we have single/none line logging, post the failed test messages
            if (Logger.Instance.IsSingleOrNoLined && ResultQueue.FailedTestResults.Any())
            {
                WriteFailedResultsToConsole();
            }
            
            //create result file and write
            if (!DisableFileOutput)
            {
                Logger.Instance.WriteDoubleLine(Environment.NewLine);

                if (Logger.Instance.IsSingleOrNoLined)
                {
                    Logger.Instance.WriteMessage(Environment.NewLine);
                }

                Logger.Instance.WriteMessage("Creating output file...");
                var file = Logger.Instance.CreateFile(OutputFolder, OutputFile, OutputType);

                if (!string.IsNullOrEmpty(file))
                {
                    WriteResultsToFile(file);
                    Logger.Instance.WriteMessage("Output file created:\n" + file);
                }
            }            
            else
            {
                Logger.Instance.WriteMessage("Output file creation disabled");
            }

            //write results and timer
            Logger.Instance.WriteDoubleLine(Environment.NewLine);
            Logger.Instance.WriteMessage(ResultQueue.ToTotalString());
            Logger.Instance.WriteMessage(string.Format("Total time: {0}", Timer.Elapsed));
            Logger.Instance.WriteDoubleLine(postcede: Environment.NewLine);

            IsRunning = false;
            return ResultQueue;
        }

        public void Interrupt()
        {
            if (ParallelThreads != default(IList<TestFixtureThread>))
            {
                foreach (var thread in ParallelThreads)
                {
                    thread.Interrupt();
                }

                Task.WaitAll(ParallelTask);
            }

            if (SingularThread != default(TestFixtureThread))
            {
                SingularThread.Interrupt();
                Task.WaitAll(SingularTask);
            }

            Logger.Instance.WriteMessage(string.Format("{1}{1}{0}", "EDISON STOPPED", Environment.NewLine));
        }
        
        #endregion

        #region Private Methods

        private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name).Name;
            var path = PathRepository.GetDirectoryName(args.RequestingAssembly.Location);
            return AssemblyRepository.LoadFrom(path + "\\" + name + ".dll");
        }

        private Exception RunGlobalSetup(Type fixture, object activator)
        {
            if (fixture == default(Type))
            {
                return default(Exception);
            }

            try
            {
                var setup = ReflectionRepository.GetMethods<SetupAttribute>(fixture);
                ReflectionRepository.Invoke(setup, activator);
            }
            catch (Exception ex)
            {
                return ex;
            }

            return default(Exception);
        }

        private void RunGlobalTeardown(Type fixture, object activator)
        {
            if (fixture == default(Type) || activator == default(object))
            {
                return;
            }

            try
            {
                var teardown = ReflectionRepository.GetMethods<TeardownAttribute>(fixture);
                ReflectionRepository.Invoke(teardown, activator);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteInnerException(ex, true);
            }
        }
                
        private void WriteResultsToFile(string file)
        {
            var results = ResultQueue.TestResults.ToList();
            var output = OutputRepositoryFactory.Get(OutputType);
            
            if (!string.IsNullOrEmpty(output.OpenTag))
            {
                Logger.Instance.WriteToFile(file, output.OpenTag + Environment.NewLine);
            }

            for (var i = 0; i < results.Count; i++)
            {
                Logger.Instance.WriteResultToFile(file, i == (results.Count - 1), results[i], output);
            }

            if (!string.IsNullOrEmpty(output.CloseTag))
            {
                Logger.Instance.WriteToFile(file, Environment.NewLine + output.CloseTag);
            }
        }

        private void WriteFailedResultsToConsole()
        {
            var failedResults = ResultQueue.FailedTestResults;
            var output = OutputRepositoryFactory.Get(OutputType);

            try
            {
                Logger.Instance.ConsoleOutputType = OutputType.Txt;
                Logger.Instance.WriteDoubleLine(Environment.NewLine, Environment.NewLine);

                foreach (var result in failedResults)
                {
                    Logger.Instance.WriteTestResult(result);
                }
            }
            finally
            {
                Logger.Instance.WriteDoubleLine(Environment.NewLine, Environment.NewLine);
                Logger.Instance.ConsoleOutputType = ConsoleOutputType;
            }
        }

        #endregion

        #region Threading

        private void RunThreads(Assembly assembly, Exception globalSetupEx)
        {
            // get all possible test fixtures
            var testFixtures = AssemblyRepository.GetTestFixtures(assembly, IncludedCategories, ExcludedCategories, Fixtures, Tests, Suite).ToList();
            if (!testFixtures.Any())
            {
                return;
            }

            #region Parallel
            // if we're running in parallel, remove any singular test fixtures
            if (NumberOfFixtureThreads > 1 && testFixtures.Count() != 1)
            {
                testFixtures = testFixtures.Where(t => ReflectionRepository.HasValidConcurrency(t, ConcurrencyType.Parallel)).OrderBy(t => t.FullName).ToList();
            }

            var fixturesCount = testFixtures.Count();
            var fixturesThreadCount = NumberOfFixtureThreads;
            if (fixturesCount < NumberOfFixtureThreads)
            {
                fixturesThreadCount = fixturesCount;
            }

            ParallelThreads = new List<TestFixtureThread>(fixturesThreadCount);
            var segment = fixturesCount == 0 ? 0 : (int)Math.Round((double)fixturesCount / (double)fixturesThreadCount, MidpointRounding.ToEven);

            // setup all the threads that are to be run in parallel
            var threadCount = 1;
            for (threadCount = 1; threadCount <= fixturesThreadCount; threadCount++)
            {
                var testFixturesSegment = threadCount == fixturesThreadCount
                    ? testFixtures.Skip((int)((threadCount - 1) * segment))
                    : testFixtures.Skip((int)((threadCount - 1) * segment)).Take((int)(segment));

                var thread = new TestFixtureThread(threadCount, this, ResultQueue, testFixturesSegment, globalSetupEx, ConcurrencyType.Parallel, NumberOfTestThreads);
                ParallelThreads.Add(thread);
            }

            // run the parallel threads
            ParallelTask = Task.Run(() => Parallel.ForEach(ParallelThreads, thread => thread.RunTestFixtures()));
            Task.WaitAll(ParallelTask);

            #endregion

            #region Singular

            // setup - if needed - the singular thread
            var singularTestFixtures = testFixtures.Where(t => ReflectionRepository.HasValidConcurrency(t, ConcurrencyType.Serial)).OrderBy(t => t.FullName).ToList();

            // run the singular thread
            if (!EnumerableHelper.IsNullOrEmpty(singularTestFixtures))
            {
                SingularThread = new TestFixtureThread(threadCount + 1, this, ResultQueue, singularTestFixtures, globalSetupEx, ConcurrencyType.Serial, NumberOfTestThreads);
                SingularTask = Task.Factory.StartNew(() => SingularThread.RunTestFixtures());
                Task.WaitAll(SingularTask);
            }

            // if an exception was thrown in global setup, return at this point
            if (globalSetupEx != default(Exception))
            {
                return;
            }

            #endregion

            #region Rerun Failed Tests

            // if enabled, and under threshold, re-run failed tests
            if (RerunFailedTests && ResultQueue.TotalFailedCount != 0 && ResultQueue.TotalCount != 0)
            {
                var percentageFailed = (int)(((double)ResultQueue.TotalFailedCount / (double)ResultQueue.TotalCount) * 100d);
                if (percentageFailed <= RerunThreshold)
                {
                    var failedTests = ResultQueue.FailedTestResults.Select(x => x.BasicName).ToList();
                    var rerunTestFixtures = AssemblyRepository.GetTestFixtures(assembly, default(IList<string>), default(IList<string>), default(IList<string>), failedTests, null).ToList();
                    SingularThread = new TestFixtureThread(threadCount + 2, this, ResultQueue, rerunTestFixtures, default(Exception), ConcurrencyType.Serial, 1);
                    SingularTask = Task.Factory.StartNew(() => SingularThread.RunTestFixtures());
                    Task.WaitAll(SingularTask);
                }
            }

            #endregion
        }

        #endregion

    }
}
