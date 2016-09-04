/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using NUnit.Framework;
using Edison.Engine.Contexts;
using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Core.Enums;
using Edison.Injector;
using Moq;
using Edison.Engine;
using System.Text;

namespace Edison.Console.Test
{
    [TestFixture]
    public class ParameterParserTests
    {

        [TearDown]
        public void Teardown()
        {
            DIContainer.Instance.Unbind<IFileRepository>();
        }

        [TestFixtureTearDown]
        public static void ClassTeardown()
        {
            DIContainer.Instance.Dispose();
        }

        #region Constructor

        [Test]
        [ExpectedException(ExpectedException = typeof(Exception), ExpectedMessage = "No EdisonContext supplied for parsing parameters", MatchType = MessageMatch.Contains)]
        public void NoEdisonContextTest()
        {
            ParameterParser.Parse(default(EdisonContext), default(string[]));
        }

        [Test]
        public void NoArgsTest()
        {
            var result = ParameterParser.Parse(EdisonContext.Create(), default(string[]));
            Assert.IsFalse(result);
        }

        #endregion

        #region Threads

        [Test]
        public void ValidNumberOfFixtureThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--ft", "2" });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.NumberOfFixtureThreads);
        }

        [Test]
        public void InvalidValueForFixtureThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                ParameterParser.Parse(context, new string[] { "--a", dll, "--ft", "-2" });
                Assert.AreEqual(-2, context.NumberOfFixtureThreads);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("Value must be greater than 0 for fixture threading", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidNumberOfTestThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--tt", "2" });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.NumberOfTestThreads);
        }

        [Test]
        public void InvalidValueForTestThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                ParameterParser.Parse(context, new string[] { "--a", dll, "--tt", "-2" });
                Assert.AreEqual(-2, context.NumberOfTestThreads);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("Value must be greater than 0 for test threading", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Assembly

        [Test]
        public void ValidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Assemblies.Count);
            Assert.AreEqual(dll, context.Assemblies[0]);
        }

        [Test]
        public void InvalidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.txt";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(false);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll });
                Assert.IsTrue(result);
                Assert.AreEqual(1, context.Assemblies.Count);
                Assert.AreEqual(dll, context.Assemblies[0]);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("Assembly is not a valid dll", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidFileForAssemblyTest()
        {
            var dll1 = "dummy/path/to.dll";
            var dll2 = "dummy/path/to2.dll";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.Exists(dll1)).Returns(true);
            fileMock.Setup(x => x.Exists(dll2)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { dll1, dll2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", file });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Assemblies.Count);
            Assert.AreEqual(file, context.Assemblies[0]);
        }

        [Test]
        public void ValidFileWithInvalidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.txt";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { dll });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", file });
                Assert.IsTrue(result);
                Assert.AreEqual(1, context.Assemblies.Count);
                Assert.AreEqual(file, context.Assemblies[0]);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("Assembly is not a valid dll", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void InvalidFileForAssemblyTest()
        {
            var dll = "dummy/path/to.dll";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { dll });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", file });
                Assert.IsTrue(result);
                Assert.AreEqual(1, context.Assemblies.Count);
                Assert.AreEqual(file, context.Assemblies[0]);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("File for list of asemblies not found", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Fixtures

        [Test]
        public void ValidFixtureTest()
        {
            var fixture = "this.is.some.fixture";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--f", fixture });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Fixtures.Count);
            Assert.AreEqual(fixture, context.Fixtures[0]);
        }

        [Test]
        public void ValidFileForFixturesTest()
        {
            var fixture1 = "this.is.some.fixture";
            var fixture2 = "this.is.some.fixture2";
            var file = "dummy/path/to/file";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { fixture1, fixture2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--f", file });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Fixtures.Count);
            Assert.AreEqual(file, context.Fixtures[0]);
        }

        [Test]
        public void InvalidFileForFixturesTest()
        {
            var dll = "dummy/path/to.dll";
            var fixture = "this.is.some.fixture";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { fixture });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--f", file });
                Assert.IsTrue(result);
                Assert.AreEqual(1, context.Fixtures.Count);
                Assert.AreEqual(file, context.Fixtures[0]);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("File for list of fixtures not found", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Tests

        [Test]
        public void ValidTestTest()
        {
            var test = "this.is.some.test";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--t", test });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Tests.Count);
            Assert.AreEqual(test, context.Tests[0]);
        }

        [Test]
        public void ValidFileForTestsTest()
        {
            var test1 = "this.is.some.test";
            var test2 = "this.is.some.test2";
            var file = "dummy/path/to/file";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { test1, test2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--t", file });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Tests.Count);
            Assert.AreEqual(file, context.Tests[0]);
        }

        [Test]
        public void InvalidFileForTestsTest()
        {
            var test = "this.is.some.test";
            var file = "dummy/path/to/file";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { test });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--t", file });
                Assert.IsTrue(result);
                Assert.AreEqual(1, context.Tests.Count);
                Assert.AreEqual(file, context.Tests[0]);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("File for list of tests not found", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Console Output Type

        [Test]
        public void ValidConsoleOutputTypeTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var type = "csv";
            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--cot", type });
            Assert.IsTrue(result);
            Assert.AreEqual(OutputType.Csv, context.ConsoleOutputType);
        }

        [Test]
        public void InvalidConsoleOutputTypeTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                Logger.Instance.Disable();
                var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--cot", "dummy" });
                Assert.IsFalse(result);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("Console output type supplied is incorrect", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Rerun Failed Tests

        [Test]
        public void ValidRerunFailedTestsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--rft" });
            Assert.IsTrue(result);
            Assert.IsTrue(context.RerunFailedTests);
        }

        [Test]
        public void DefaultRerunFailedTestsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            Assert.IsTrue(result);
            Assert.IsFalse(context.RerunFailedTests);
        }

        #endregion

        #region Rerun Threshold

        [Test]
        public void ValidRerunThresholdTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var threshold = 33;
            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--rt", threshold.ToString() });
            Assert.IsTrue(result);
            Assert.AreEqual(threshold, context.RerunThreshold);
        }

        [Test]
        public void DefaultRerunThresholdTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            Assert.IsTrue(result);
            Assert.AreEqual(100, context.RerunThreshold);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(101)]
        public void InvalidRerunThresholdTest(int threshold)
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--rt", threshold.ToString() });
                Assert.IsTrue(result);
                Assert.AreEqual(threshold, context.RerunThreshold);
            }
            catch (ParseException ex)
            {
                StringAssert.IsMatch(@"Value must be (less|greater) than or equal to \d+ for re-run threshold, but got '-*\d+'", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Suite Tests

        [Test]
        public void ValidSuiteTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            var suite = "some name";

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--s", suite });
            Assert.IsTrue(result);
            Assert.AreEqual(suite, context.Suite);
        }

        [Test]
        public void DefaultSuiteTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            Assert.IsTrue(result);
            Assert.IsNull(context.Suite);
        }

        #endregion

        #region Edisonfile

        [Test]
        public void EdisonfileDoesntExist()
        {
            var ef = "dummy/Edisonfile";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(ef)).Returns(false);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--ef", ef });
            Assert.IsFalse(result);
        }

        #endregion

        #region Solution Tests

        [Test]
        public void ValidSlnForSolutionTest()
        {
            var sln = "dummy/path/to.sln";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--sln", sln });
            Assert.IsTrue(result);
            Assert.AreEqual(sln, context.Solution);
            Assert.AreEqual(0, context.Assemblies.Count);
        }

        [Test]
        public void InvalidSlnForSolutionTest()
        {
            var sln = "dummy/path/to.txt";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(false);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--sln", sln });
                Assert.IsTrue(result);
                Assert.AreEqual(sln, context.Solution);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("Solution is not a valid sln file", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Test]
        public void ValidSolutionAndAssemblyTest()
        {
            var sln = "dummy/path/to.sln";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(true);
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--sln", sln });
            Assert.IsTrue(result);
            Assert.AreEqual(sln, context.Solution);
            Assert.AreEqual(1, context.Assemblies.Count);
            Assert.AreEqual(dll, context.Assemblies[0]);
        }

        [Test]
        public void InvalidSolutionAndAssemblyTest()
        {
            var fileMock = new Mock<IFileRepository>();
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--f", "random" });
                Assert.IsTrue(result);
                Assert.AreEqual(0, context.Assemblies.Count);
                Assert.IsNullOrEmpty(context.Solution);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains("No assembly or solution paths supplied", ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Solution Configuration Tests

        [Test]
        public void ValidSolutionConfigTest()
        {
            var sln = "dummy/path/to.sln";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            var config = "SomeConfig";

            var result = ParameterParser.Parse(context, new string[] { "--sln", sln, "--sconfig", config });
            Assert.IsTrue(result);
            Assert.AreEqual(config, context.SolutionConfiguration);
        }

        [Test]
        public void DefaultSolutionConfigTest()
        {
            var sln = "dummy/path/to.sln";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();

            var result = ParameterParser.Parse(context, new string[] { "--sln", sln });
            Assert.IsTrue(result);
            Assert.AreEqual("Debug", context.SolutionConfiguration);
        }

        #endregion

    }
}
