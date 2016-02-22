﻿/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edison.Engine.Contexts;
using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Core.Enums;
using Edison.Injector;
using Moq;

namespace Edison.Console.Test
{
    [TestClass]
    public class ParameterParserTests
    {

        [TestCleanup]
        public void Teardown()
        {
            DIContainer.Instance.Unbind<IFileRepository>();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            DIContainer.Instance.Dispose();
        }

        #region Constructor

        [TestMethod]
        [ExpectedException(typeof(Exception), "No EdisonContext supplied for parsing parameters")]
        public void NoEdisonContextTest()
        {
            ParameterParser.Parse(default(EdisonContext), default(string[]));
        }

        [TestMethod]
        public void NoArgsTest()
        {
            var result = ParameterParser.Parse(new EdisonContext(), default(string[]));
            Assert.IsFalse(result);
        }

        #endregion

        #region Threads

        [TestMethod]
        public void ValidNumberOfFixtureThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();
            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--ft", "2" });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.NumberOfFixtureThreads);
        }

        [TestMethod]
        public void InvalidValueForFixtureThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            try
            {
                ParameterParser.Parse(context, new string[] { "--a", dll, "--ft", "-2" });
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "Value must be greater than 0 for fixture threading");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ValidNumberOfTestThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();
            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--tt", "2" });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.NumberOfTestThreads);
        }

        [TestMethod]
        public void InvalidValueForTestThreadsTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            try
            {
                ParameterParser.Parse(context, new string[] { "--a", dll, "--tt", "-2" });
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "Value must be greater than 0 for test threading");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Assembly

        [TestMethod]
        public void ValidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();
            
            var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.AssemblyPaths.Count);
            Assert.AreEqual(dll, context.AssemblyPaths[0]);
        }

        [TestMethod]
        public void InvalidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.txt";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(false);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();
            
            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "Assembly it not a valid dll");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void ValidFileForAssemblyTest()
        {
            var dll1 = "dummy/path/to.dll";
            var dll2 = "dummy/path/to2.dll";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.Exists(dll1)).Returns(true);
            fileMock.Setup(x => x.Exists(dll2)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file)).Returns(new string[] { dll1, dll2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            var result = ParameterParser.Parse(context, new string[] { "--a", file });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.AssemblyPaths.Count);
            Assert.AreEqual(dll1, context.AssemblyPaths[0]);
            Assert.AreEqual(dll2, context.AssemblyPaths[1]);
        }

        [TestMethod]
        public void ValidFileWithInvalidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.txt";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file)).Returns(new string[] { dll });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);
            
            var context = new EdisonContext();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", file });
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "Assembly it not a valid dll");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public void InvalidFileForAssemblyTest()
        {
            var dll = "dummy/path/to.dll";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.ReadAllLines(file)).Returns(new string[] { dll });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);
            
            var context = new EdisonContext();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", file });
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "File for list of asemblies not found");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Fixtures

        [TestMethod]
        public void ValidFixtureTest()
        {
            var fixture = "this.is.some.fixture";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--f", fixture });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Fixtures.Count);
            Assert.AreEqual(fixture, context.Fixtures[0]);
        }

        [TestMethod]
        public void ValidFileForFixturesTest()
        {
            var fixture1 = "this.is.some.fixture";
            var fixture2 = "this.is.some.fixture2";
            var file = "dummy/path/to/file";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file)).Returns(new string[] { fixture1, fixture2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);
            
            var context = new EdisonContext();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--f", file });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.Fixtures.Count);
            Assert.AreEqual(fixture1, context.Fixtures[0]);
            Assert.AreEqual(fixture2, context.Fixtures[1]);
        }

        [TestMethod]
        public void InvalidFileForFixturesTest()
        {
            var dll = "dummy/path/to.dll";
            var fixture = "this.is.some.fixture";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file)).Returns(new string[] { fixture });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);
            
            var context = new EdisonContext();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--f", file });
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "File for list of fixtures not found");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Fixtures

        [TestMethod]
        public void ValidTestTest()
        {
            var test = "this.is.some.test";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--t", test });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Tests.Count);
            Assert.AreEqual(test, context.Tests[0]);
        }

        [TestMethod]
        public void ValidFileForTestsTest()
        {
            var test1 = "this.is.some.test";
            var test2 = "this.is.some.test2";
            var file = "dummy/path/to/file";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file)).Returns(new string[] { test1, test2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--t", file });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.Tests.Count);
            Assert.AreEqual(test1, context.Tests[0]);
            Assert.AreEqual(test2, context.Tests[1]);
        }

        [TestMethod]
        public void InvalidFileForTestsTest()
        {
            var test = "this.is.some.test";
            var file = "dummy/path/to/file";
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file)).Returns(new string[] { test });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--ts", file });
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "File for list of tests not found");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Console Output Type

        [TestMethod]
        public void ValidConsoleOutputTypeTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();

            var type = "csv";
            var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--cot", type });
            Assert.IsTrue(result);
            Assert.AreEqual(OutputType.Csv, context.ConsoleOutputType);
        }

        [TestMethod]
        public void InvalidConsoleOutputTypeTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = new EdisonContext();
            
            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll, "--cot", "dummy" });
                Assert.IsFalse(result);
            }
            catch (ParseException ex)
            {
                StringAssert.Contains(ex.Message, "Console output type supplied is incorrect");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

    }
}
