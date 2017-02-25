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
using Edison.Engine.Events;
using Edison.Engine.Repositories.Interfaces;
using Edison.Injector;
using System.Threading.Tasks;
using Edison.Engine.Utilities.Helpers;
using Edison.Engine.Repositories.Outputs;

namespace Edison.Engine.Contexts
{
    [Serializable]
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

        private IDirectoryRepository DirectoryRepository
        {
            get { return DIContainer.Instance.Get<IDirectoryRepository>(); }
        }

        private IAppDomainRepository AppDomainRepository
        {
            get { return DIContainer.Instance.Get<IAppDomainRepository>(); }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the assembly paths.
        /// </summary>
        /// <value>
        /// The assembly paths.
        /// </value>
        public List<string> Assemblies { get; set; }

        [Obsolete("This will be deprecated in future releases, use the Assemblies property instead.")]
        public List<string> AssemblyPaths
        {
            get { return Assemblies; }
            set { Assemblies = value; }
        }

        /// <summary>
        /// Gets or sets the included categories.
        /// </summary>
        /// <value>
        /// The included categories.
        /// </value>
        public List<string> IncludedCategories { get; set; }

        /// <summary>
        /// Gets or sets the excluded categories.
        /// </summary>
        /// <value>
        /// The excluded categories.
        /// </value>
        public List<string> ExcludedCategories { get; set; }

        /// <summary>
        /// Gets or sets the fixtures.
        /// </summary>
        /// <value>
        /// The fixtures.
        /// </value>
        public List<string> Fixtures { get; set; }

        /// <summary>
        /// Gets the tests.
        /// </summary>
        /// <value>
        /// The tests.
        /// </value>
        public List<string> Tests { get; set; }

        /// <summary>
        /// Gets or sets the number of fixture threads.
        /// </summary>
        /// <value>
        /// The number of fixture threads.
        /// </value>
        public int NumberOfFixtureThreads { get; set; }

        /// <summary>
        /// Gets or sets the number of test threads.
        /// </summary>
        /// <value>
        /// The number of test threads.
        /// </value>
        public int NumberOfTestThreads { get; set; }

        /// <summary>
        /// Gets or sets the output file name.
        /// </summary>
        /// <value>
        /// The output file.
        /// </value>
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the output directory path.
        /// </summary>
        /// <value>
        /// The output folder.
        /// </value>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the type of the output.
        /// </summary>
        /// <value>
        /// The type of the output.
        /// </value>
        public OutputType OutputType { get; set; }

        /// <summary>
        /// Gets or sets the type of the console output.
        /// </summary>
        /// <value>
        /// The type of the console output.
        /// </value>
        public OutputType ConsoleOutputType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether file output should be disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if file output is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisableFileOutput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether console output should be disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if console output is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisableConsoleOutput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether test output should be disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if test output is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool DisableTestOutput { get; set; }

        /// <summary>
        /// Gets or sets the test result URL.
        /// </summary>
        /// <value>
        /// The test result URL.
        /// </value>
        public string TestResultURL { get; set; }

        /// <summary>
        /// Gets or sets the test run identifier.
        /// </summary>
        /// <value>
        /// The test run identifier.
        /// </value>
        public string TestRunId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the context should re-run failed tests.
        /// </summary>
        /// <value>
        ///   <c>true</c> if re-run failed tests; otherwise, <c>false</c>.
        /// </value>
        public bool RerunFailedTests { get; set; }

        /// <summary>
        /// Gets or sets the re-run threshold value.
        /// </summary>
        /// <value>
        /// The rerun threshold.
        /// </value>
        public int RerunThreshold { get; set; }

        /// <summary>
        /// Gets or sets the test suite.
        /// </summary>
        /// <value>
        /// The suite.
        /// </value>
        public string Suite { get; set; }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        /// <value>
        /// The solution.
        /// </value>
        public string Solution { get; set; }

        /// <summary>
        /// Gets or sets the solution configuration.
        /// </summary>
        /// <value>
        /// The solution configuration.
        /// </value>
        public string SolutionConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the slack token.
        /// </summary>
        /// <value>
        /// The slack token.
        /// </value>
        public string SlackToken { get; set; }

        #endregion

        #region Public Readonly Properties

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the current assembly that is currently having its tests executed.
        /// </summary>
        /// <value>
        /// The current assembly.
        /// </value>
        public string CurrentAssembly { get; private set; }

        #endregion

        #region Private Fields

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

        /// <summary>
        /// Initializes a new instance of the <see cref="EdisonContext"/> class.
        /// </summary>
        private EdisonContext()
        {
            Assemblies = new List<string>(1);
            IncludedCategories = new List<string>();
            ExcludedCategories = new List<string>();
            Fixtures = new List<string>();
            Tests = new List<string>();
            NumberOfFixtureThreads = 1;
            NumberOfTestThreads = 1;
            ConsoleOutputType = OutputType.Txt;
            OutputType = OutputType.Json;
            OutputDirectory = Environment.CurrentDirectory;
            OutputFile = "ResultFile";
            TestResultURL = string.Empty;
            IsRunning = true;
            RerunThreshold = 100;
        }

        /// <summary>
        /// Creates an instance of an EdisonContext.
        /// </summary>
        /// <returns></returns>
        public static EdisonContext Create()
        {
            return new EdisonContext();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public void Validate()
        {
            ContextValidator.Validate(this);
        }

        /// <summary>
        /// Runs this instance after passing validation, executing tests in the passed assemblies/solution.
        /// </summary>
        /// <returns></returns>
        public TestResultDictionary Run()
        {
            //run validation first
            Validate();

            //start timer
            var timer = new Stopwatch();
            timer.Start();

            //set logging output
            SetupLogging();

            //set output logging type
            Logger.Instance.ConsoleOutputType = ConsoleOutputType;

            //create results queue/list
            ResultQueue = new TestResultDictionary(this);

            //bind test result events
            if (OnTestResult != default(TestResultEventHandler))
            {
                ResultQueue.OnTestResult += OnTestResult;
            }

            //loop through all assemblies, running their tests
            RunAssemblies();

            //stop the timer
            timer.Stop();

            //if we have single/none line logging, post the failed test messages
            if (Logger.Instance.IsSingleOrNoLined && ResultQueue.FailedTestResults.Any())
            {
                WriteFailedResultsToConsole();
            }

            //create result file and write
            WriteResultsToFile();

            //write results and timer
            Logger.Instance.WriteDoubleLine(Environment.NewLine);
            Logger.Instance.WriteMessage(ResultQueue.ToTotalString());
            Logger.Instance.WriteMessage(string.Format("Total time: {0}", timer.Elapsed));
            Logger.Instance.WriteDoubleLine(postcede: Environment.NewLine);

            IsRunning = false;
            return ResultQueue;
        }

        /// <summary>
        /// Interrupts this instance.
        /// </summary>
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
            var path = PathRepository.GetDirectoryName(args.RequestingAssembly == null ? "." : args.RequestingAssembly.Location);
            return AssemblyRepository.LoadFrom(path + "\\" + name + ".dll");
        }

        private void RunAssemblies()
        {
            //remove any duplicate assembly paths
            var assemblies = Assemblies.Distinct();

            //bind assembly event resolver
            AppDomainRepository.CurrentDomain.AssemblyResolve += ResolveAssembly;

            //loop through all assemblies, running their tests
            foreach (var assemblyPath in assemblies)
            {
                CurrentAssembly = PathRepository.GetFileName(assemblyPath);

                var assembly = AssemblyRepository.LoadFile(assemblyPath);
                AppDomainRepository.SetAppConfig(PathRepository.GetFullPath(assemblyPath) + ".config");

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
        }

        private void SetupLogging()
        {
            if (DisableConsoleOutput)
            {
                Logger.Instance.Disable();
                Logger.Instance.DisableConsole();
            }

            if (DisableTestOutput)
            {
                Logger.Instance.DisableConsole();
            }

            Logger.Instance.ConsoleOutputType = ConsoleOutputType;
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

        private void WriteResultsToFile()
        {
            if (DisableFileOutput)
            {
                Logger.Instance.WriteMessage("Output file creation disabled");
                return;
            }

            Logger.Instance.WriteDoubleLine(Environment.NewLine);

            if (Logger.Instance.IsSingleOrNoLined)
            {
                Logger.Instance.WriteMessage(Environment.NewLine);
            }

            Logger.Instance.WriteMessage("Creating output file...");
            var file = Logger.Instance.CreateFile(OutputDirectory, OutputFile, OutputType);

            if (!string.IsNullOrWhiteSpace(file))
            {
                var results = ResultQueue.TestResults.ToList();
                var output = OutputRepositoryFactory.Get(OutputType);

                if (!string.IsNullOrWhiteSpace(output.OpenTag))
                {
                    Logger.Instance.WriteToFile(file, output.OpenTag + Environment.NewLine);
                }

                for (var i = 0; i < results.Count; i++)
                {
                    Logger.Instance.WriteResultToFile(file, i == (results.Count - 1), results[i], output);
                }

                if (!string.IsNullOrWhiteSpace(output.CloseTag))
                {
                    Logger.Instance.WriteToFile(file, Environment.NewLine + output.CloseTag);
                }

                Logger.Instance.WriteMessage(string.Format("Output file created: {0}{1}", file, Environment.NewLine));
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
            var singularTestFixtures = default(List<Type>);

            if (!testFixtures.Any())
            {
                return;
            }

            #region Parallel
            // if we're running in parallel, remove any singular test fixtures
            if (NumberOfFixtureThreads > 1 && testFixtures.Count() != 1)
            {
                singularTestFixtures = testFixtures.Where(t => ReflectionRepository.HasValidConcurrency(t, ConcurrencyType.Serial)).OrderBy(t => t.FullName).ToList();
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
            if (NumberOfFixtureThreads > 1 && !EnumerableHelper.IsNullOrEmpty(singularTestFixtures))
            {
                SingularThread = new TestFixtureThread(threadCount, this, ResultQueue, singularTestFixtures, globalSetupEx, ConcurrencyType.Serial, NumberOfTestThreads);
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
                var percentageFailed = (int)ResultQueue.FailureRate;

                if (percentageFailed <= RerunThreshold)
                {
                    var failedTests = ResultQueue.FailedTestResults.Select(x => x.BasicName).ToList();
                    var rerunTestFixtures = AssemblyRepository.GetTestFixtures(assembly, default(IList<string>), default(IList<string>), default(IList<string>), failedTests, null).ToList();
                    SingularThread = new TestFixtureThread(threadCount + 1, this, ResultQueue, rerunTestFixtures, default(Exception), ConcurrencyType.Serial, 1);
                    SingularTask = Task.Factory.StartNew(() => SingularThread.RunTestFixtures());
                    Task.WaitAll(SingularTask);
                }
            }

            #endregion
        }

        #endregion

    }
}
