/*
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
using System.Reflection;
using Edison.Console.Test.TestRepositories;
using Edison.Engine.Core.Enums;
using Edison.Injector;
using System.Collections.Generic;

namespace Edison.Console.Test
{
    [TestClass]
    public class ParameterParserTests
    {

        [TestInitialize]
        public void Setup()
        {
            DIContainer.Instance.Bind<IFileRepository, MockFileRepository>();
        }

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
            var context = new EdisonContext();
            var result = ParameterParser.Parse(context, new string[] { "--ft", "2" });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.NumberOfFixtureThreads);
        }

        [TestMethod]
        public void InvalidValueForFixtureThreadsTest()
        {
            var context = new EdisonContext();

            try
            {
                ParameterParser.Parse(context, new string[] { "--ft", "-2" });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "Value must be greater than 0 for fixture threading");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TooManyValuesForFixtureThreadsTest()
        {
            var context = new EdisonContext();

            try
            {
                ParameterParser.Parse(context, new string[] { "--ft", "2", "1" });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "Incorrect number of arguments supplied");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ValidNumberOfTestThreadsTest()
        {
            var context = new EdisonContext();
            var result = ParameterParser.Parse(context, new string[] { "--tt", "2" });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.NumberOfTestThreads);
        }

        [TestMethod]
        public void InvalidValueForTestThreadsTest()
        {
            var context = new EdisonContext();

            try
            {
                ParameterParser.Parse(context, new string[] { "--tt", "-2" });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "Value must be greater than 0 for test threading");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TooManyValuesForTestThreadsTest()
        {
            var context = new EdisonContext();

            try
            {
                ParameterParser.Parse(context, new string[] { "--tt", "2", "1" });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "Incorrect number of arguments supplied");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

        #region Assembly

        [TestMethod]
        public void ValidDllForAssemblyTest()
        {
            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>() { { "exists", true } });

            var context = new EdisonContext();

            var dll = "dummy/path/to.dll";
            var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.AssemblyPaths.Count);
            Assert.AreEqual(dll, context.AssemblyPaths[0]);
        }

        [TestMethod]
        public void InvalidDllForAssemblyTest()
        {
            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>() { { "exists", false } });
            var context = new EdisonContext();

            var dll = "dummy/path/to.txt";

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", dll });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "Assembly it not a valid dll");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ValidFileForAssemblyTest()
        {
            var dll1 = "dummy/path/to.dll";
            var dll2 = "dummy/path/to2.dll";
            
            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>()
            {
                { "exists", true },
                { "readAllLines", new string[] { dll1, dll2 } }
            });

            var context = new EdisonContext();
            var file = "dummy/path/to/file";

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

            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>()
            {
                { "exists", true },
                { "readAllLines", new string[] { dll } }
            });

            var context = new EdisonContext();
            var file = "dummy/path/to/file";

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", file });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "Assembly it not a valid dll");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void InvalidFileForAssemblyTest()
        {
            var dll = "dummy/path/to.dll";

            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>()
            {
                { "exists", false },
                { "readAllLines", new string[] { dll } }
            });

            var context = new EdisonContext();
            var file = "dummy/path/to/file";

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--a", file });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "File for list of asemblies not found");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

        #region Fixtures

        [TestMethod]
        public void ValidFixtureTest()
        {
            var context = new EdisonContext();

            var fixture = "this.is.some.fixture";
            var result = ParameterParser.Parse(context, new string[] { "--f", fixture });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Fixtures.Count);
            Assert.AreEqual(fixture, context.Fixtures[0]);
        }

        [TestMethod]
        public void ValidFileForFixturesTest()
        {
            var fixture1 = "this.is.some.fixture";
            var fixture2 = "this.is.some.fixture2";

            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>()
            {
                { "exists", true },
                { "readAllLines", new string[] { fixture1, fixture2 } }
            });

            var context = new EdisonContext();
            var file = "dummy/path/to/file";

            var result = ParameterParser.Parse(context, new string[] { "--f", file });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.Fixtures.Count);
            Assert.AreEqual(fixture1, context.Fixtures[0]);
            Assert.AreEqual(fixture2, context.Fixtures[1]);
        }

        [TestMethod]
        public void InvalidFileForFixturesTest()
        {
            var fixture = "this.is.some.fixture";

            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>()
            {
                { "exists", false },
                { "readAllLines", new string[] { fixture } }
            });
            
            var context = new EdisonContext();
            var file = "dummy/path/to/file";

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--f", file });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "File for list of fixtures not found");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

        #region Fixtures

        [TestMethod]
        public void ValidTestTest()
        {
            var context = new EdisonContext();

            var test = "this.is.some.test";
            var result = ParameterParser.Parse(context, new string[] { "--ts", test });
            Assert.IsTrue(result);
            Assert.AreEqual(1, context.Tests.Count);
            Assert.AreEqual(test, context.Tests[0]);
        }

        [TestMethod]
        public void ValidFileForTestsTest()
        {
            var test1 = "this.is.some.test";
            var test2 = "this.is.some.test2";

            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>()
            {
                { "exists", true },
                { "readAllLines", new string[] { test1, test2 } }
            });

            var context = new EdisonContext();
            var file = "dummy/path/to/file";

            var result = ParameterParser.Parse(context, new string[] { "--ts", file });
            Assert.IsTrue(result);
            Assert.AreEqual(2, context.Tests.Count);
            Assert.AreEqual(test1, context.Tests[0]);
            Assert.AreEqual(test2, context.Tests[1]);
        }

        [TestMethod]
        public void InvalidFileForTestsTest()
        {
            var test = "this.is.some.test";

            DIContainer.Instance.BindAndCache<IFileRepository, MockFileRepository>(new Dictionary<string, object>()
            {
                { "exists", false },
                { "readAllLines", new string[] { test } }
            });

            var context = new EdisonContext();
            var file = "dummy/path/to/file";

            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--ts", file });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "File for list of tests not found");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

        #region Console Output Type

        [TestMethod]
        public void ValidConsoleOutputTypeTest()
        {
            var context = new EdisonContext();

            var type = "csv";
            var result = ParameterParser.Parse(context, new string[] { "--cot", type });
            Assert.IsTrue(result);
            Assert.AreEqual(OutputType.Csv, context.ConsoleOutputType);
        }

        [TestMethod]
        public void InvalidConsoleOutputTypeTest()
        {
            var context = new EdisonContext();
            
            try
            {
                var result = ParameterParser.Parse(context, new string[] { "--cot", "dummy" });
            }
            catch (TargetInvocationException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.IsInstanceOfType(ex.InnerException, typeof(ParseException));
                StringAssert.Contains(ex.InnerException.Message, "Console output type supplied is incorrect");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        #endregion

    }
}
