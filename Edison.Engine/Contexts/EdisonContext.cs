/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Edison.Engine.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Edison.Engine.Utilities.Helpers;
using System.Threading;
using Edison.Engine.Threading;
using Edison.Engine.Core.Enums;
using Edison.Engine.Utilities.Structures;
using System.Diagnostics;
using Edison.Engine.Core.Output;

namespace Edison.Engine.Contexts
{
    public class EdisonContext
    {

        #region Properties
        
        public List<string> AssemblyPaths { get; private set; }
        public List<string> IncludedCategories { get; private set; }
        public List<string> ExcludedCategories { get; private set; }
        public List<string> Fixtures { get; private set; }
        public int NumberOfThreads { get; set; }
        public string OutputFile { get; set; }
        public string OutputFolder { get; set; }
        public OutputType OutputType { get; set; }
        public bool CreateOutput { get; set; }
        public string TestResultURL { get; set; }
        public string TestRunId { get; set; }

        private Stopwatch Timer = default(Stopwatch);
        private TestResultDictionary ResultQueue = default(TestResultDictionary);

        private IList<EdisonTestThread> Threads = default(IList<EdisonTestThread>);
        private EdisonTestThread SingularThread = default(EdisonTestThread);

        #endregion

        #region Constructor

        public EdisonContext()
        {
            AssemblyPaths = new List<string>(1);
            IncludedCategories = new List<string>();
            ExcludedCategories = new List<string>();
            Fixtures = new List<string>();
            NumberOfThreads = 1;
            OutputType = OutputType.Txt;
            OutputFolder = Environment.CurrentDirectory;
            OutputFile = "ResultFile";
            CreateOutput = true;
            TestResultURL = string.Empty;
        }

        #endregion

        #region Public Methods

        public void Run()
        {
            //start timer
            Timer = new Stopwatch();
            Timer.Start();

            //create queue
            ResultQueue = new TestResultDictionary(this);
            
            foreach (var assemblyPath in AssemblyPaths)
            {
                var assembly = Assembly.LoadFile(Path.GetFullPath(assemblyPath));

                //global setup
                var globalSetupFixture = assembly.GetTypes<SetupFixtureAttribute>().SingleOrDefault();
                var globalActivator = Activator.CreateInstance(globalSetupFixture, null);
                var globalSetupEx = RunGlobalSetup(globalSetupFixture, globalActivator);

                //test fixtures an threads
                SetupThreads(assembly, globalSetupEx);

                //global teardown
                RunGlobalTeardown(globalSetupFixture, globalActivator);
            }

            Timer.Stop();
            
            //create result file and write
            if (CreateOutput)
            {
                Logger.WriteDoubleLine(Environment.NewLine);
                Logger.WriteMessage("Creating output file...");
                var file = Logger.CreateFile(OutputFolder, OutputFile, OutputType);
                WriteResultsToFile(file);
                Logger.WriteMessage("Output file created:\n" + file);
            }            
            else
            {
                Logger.WriteMessage("Output file creation disabled");
            }

            //write results and timer
            Logger.WriteDoubleLine(Environment.NewLine);
            Logger.WriteMessage(ResultQueue.ToTotalString());
            Logger.WriteMessage(string.Format("Total time: {0}", Timer.Elapsed));
            Logger.WriteDoubleLine(postcede: Environment.NewLine);
        }
        
        #endregion

        #region Private Methods

        private Exception RunGlobalSetup(Type fixture, object activator)
        {
            if (fixture == default(Type))
            {
                return default(Exception);
            }

            try
            {
                var setup = fixture.GetMethods<SetupAttribute>().ToList();
                ReflectionHelper.Invoke(setup, activator);
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
                var teardown = fixture.GetMethods<TeardownAttribute>().ToList();
                ReflectionHelper.Invoke(teardown, activator);
            }
            catch (Exception ex)
            {
                Logger.WriteInnerException(ex, true);
            }
        }

        private void SetupThreads(Assembly assembly, Exception globalSetupEx)
        {
            var testFixtures = assembly.GetTypes<TestFixtureAttribute>(IncludedCategories, ExcludedCategories)
                                .Where(t => Fixtures.Count == 0 || Fixtures.Contains(t.FullName))
                                .ToList()
                                .OrderBy(t => t.FullName);

            Threads = new List<EdisonTestThread>(NumberOfThreads);

            // if we're running in parallel, remove any singular test fixtures
            var singularTestFixtures = default(IOrderedEnumerable<Type>);
            if (NumberOfThreads > 1 && testFixtures.Count() != 1)
            {
                singularTestFixtures = testFixtures.Where(t => ReflectionHelper.HasValidConcurrency(t.GetCustomAttributes(), ConcurrencyType.Serial)).OrderBy(t => t.FullName);
                testFixtures = testFixtures.Where(t => ReflectionHelper.HasValidConcurrency(t.GetCustomAttributes(), ConcurrencyType.Parallel)).OrderBy(t => t.FullName);
            }

            var fixtures = testFixtures.Count();
            if (fixtures <= NumberOfThreads)
            {
                NumberOfThreads = fixtures;
            }

            var segment = fixtures == 0 ? 0 : (double)fixtures / (double)NumberOfThreads;

            // setup all the threads that are to be run in parallel
            var i = 1;
            for (i = 1; i <= NumberOfThreads; i++)
            {
                var testFixturesSegment = i == NumberOfThreads
                    ? testFixtures.Skip((int)((i - 1) * segment)).ToList()
                    : testFixtures.Skip((int)((i - 1) * segment)).Take((int)(i * segment)).ToList();

                var thread = new EdisonTestThread(i, this, ResultQueue, testFixturesSegment, globalSetupEx);
                Threads.Add(thread);
            }

            // setup - if needed - the singular thread
            SingularThread = singularTestFixtures != default(IOrderedEnumerable<Type>)
                ? new EdisonTestThread(i + 1, this, ResultQueue, singularTestFixtures.ToList(), globalSetupEx)
                : default(EdisonTestThread);

            RunThreads();
        }

        private void RunThreads()
        {
            foreach (var thread in Threads)
            {
                thread.Start();
            }

            // keep polling the threads, so we know when they're finished
            while (Threads.Any(x => !x.IsFinished))
            {
                Thread.Sleep(2000);
            }

            // once finished, we need to run the possible singular tests
            if (SingularThread != default(EdisonTestThread))
            {
                SingularThread.Start();

                // now keep polling again, so we know when it's finished
                while (Threads.Any(x => !x.IsFinished))
                {
                    Thread.Sleep(2000);
                }
            }            
        }

        private void WriteResultsToFile(string file)
        {
            var results = ResultQueue.Values.ToList();
            var output = OutputRepositoryManager.Get(OutputType);
            
            if (!string.IsNullOrEmpty(output.OpenTag))
            {
                Logger.WriteToFile(file, output.OpenTag + Environment.NewLine);
            }

            for (var i = 0; i < results.Count; i++)
            {
                Logger.WriteResultToFile(file, i == (results.Count - 1), results[i], output);
            }

            if (!string.IsNullOrEmpty(output.CloseTag))
            {
                Logger.WriteToFile(file, Environment.NewLine + output.CloseTag);
            }
        }

        #endregion

    }
}
