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
using Edison.Engine.Utilities.Helpers;
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

        #endregion

        #region Properties

        public bool IsRunning { get; private set; }

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

        private Stopwatch Timer = default(Stopwatch);
        private TestResultDictionary ResultQueue;

        private IList<TestFixtureThread> Threads = default(IList<TestFixtureThread>);
        private IList<Task> Tasks = default(IList<Task>);
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

            Logger.Instance.ConsoleOutputType = ConsoleOutputType;

            //start timer
            Timer = new Stopwatch();
            Timer.Start();

            //create queue
            ResultQueue = new TestResultDictionary(this);

            if (OnTestResult != default(TestResultEventHandler))
            {
                ResultQueue.OnTestResult += OnTestResult;
            }

            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            foreach (var assemblyPath in AssemblyPaths)
            {
                var assembly = AssemblyRepository.LoadFile(assemblyPath);

                //global setup
                var globalSetupFixture = AssemblyRepository.GetTypes<SetupFixtureAttribute>(assembly).SingleOrDefault();
                var globalActivator = default(object);
                var globalSetupEx = default(Exception);

                if (globalSetupFixture != default(Type))
                {
                    globalActivator = Activator.CreateInstance(globalSetupFixture, null);
                    globalSetupEx = RunGlobalSetup(globalSetupFixture, globalActivator);
                }

                //test fixtures an threads
                SetupThreads(assembly, globalSetupEx);

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
            if (Threads != default(IList<TestFixtureThread>))
            {
                foreach (var thread in Threads)
                {
                    thread.Interrupt();
                }

                Task.WaitAll(Tasks.ToArray());
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

        private void SetupThreads(Assembly assembly, Exception globalSetupEx)
        {
            var testFixtures = AssemblyRepository.GetTestFixtures(assembly, IncludedCategories, ExcludedCategories, Fixtures, Tests);

            // if we're running in parallel, remove any singular test fixtures
            var singularTestFixtures = default(IOrderedEnumerable<Type>);
            if (NumberOfFixtureThreads > 1 && testFixtures.Count() != 1)
            {
                singularTestFixtures = testFixtures.Where(t => ReflectionHelper.HasValidConcurrency(t.GetCustomAttributes(), ConcurrencyType.Serial)).OrderBy(t => t.FullName);
                testFixtures = testFixtures.Where(t => ReflectionHelper.HasValidConcurrency(t.GetCustomAttributes(), ConcurrencyType.Parallel)).OrderBy(t => t.FullName);
            }

            var fixtures = testFixtures.Count();
            if (fixtures < NumberOfFixtureThreads)
            {
                NumberOfFixtureThreads = fixtures;
            }

            Threads = new List<TestFixtureThread>(NumberOfFixtureThreads);
            var segment = fixtures == 0 ? 0 : (double)fixtures / (double)NumberOfFixtureThreads;

            // setup all the threads that are to be run in parallel
            var i = 1;
            for (i = 1; i <= NumberOfFixtureThreads; i++)
            {
                var testFixturesSegment = i == NumberOfFixtureThreads
                    ? testFixtures.Skip((int)((i - 1) * segment))
                    : testFixtures.Skip((int)((i - 1) * segment)).Take((int)(segment));

                var thread = new TestFixtureThread(i, this, ResultQueue, testFixturesSegment, globalSetupEx, ConcurrencyType.Parallel, NumberOfTestThreads);
                Threads.Add(thread);
            }

            // setup - if needed - the singular thread
            SingularThread = singularTestFixtures != default(IOrderedEnumerable<Type>)
                ? new TestFixtureThread(i + 1, this, ResultQueue, singularTestFixtures, globalSetupEx, ConcurrencyType.Serial, NumberOfTestThreads)
                : default(TestFixtureThread);

            RunThreads();
        }

        private void RunThreads()
        {
            Tasks = new List<Task>(Threads.Count);
            foreach (var thread in Threads)
            {
                Tasks.Add(Task.Factory.StartNew(() => thread.RunTestFixtures()));
            }

            Task.WaitAll(Tasks.ToArray());

            // once finished, we need to run the possible singular tests
            if (SingularThread != default(TestFixtureThread))
            {
                SingularTask = Task.Factory.StartNew(() => SingularThread.RunTestFixtures());
                Task.WaitAll(SingularTask);
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

    }
}
