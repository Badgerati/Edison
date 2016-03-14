/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Engine.Contexts;
using Edison.Engine.Core.Exceptions;
using Edison.Engine.Repositories.Interfaces;
using Edison.Engine.Validators;
using Edison.Injector;
using Moq;
using NUnit.Framework;
using System.Text;

namespace Edison.Engine.Test.Contexts
{
    [TestFixture]
    public class ContextValidatorTests
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
        [ExpectedException(ExpectedException = typeof(ValidationException), ExpectedMessage = "No assembly or solution paths were supplied.")]
        public void DefaultConstructor_ValidationFails()
        {
            var context = EdisonContext.Create();
            ContextValidator.Validate(context);
        }

        #endregion

        #region Assemblies

        [Test]
        public void ValidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Assemblies.Add(dll);

            ContextValidator.Validate(context, new AssemblyValidator());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ValidationException), ExpectedMessage = "Assembly is not a valid .dll file: 'dummy/path/to.txt'")]
        public void InvalidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.txt";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(dll)).Returns(false);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);
            
            var context = EdisonContext.Create();
            context.Assemblies.Add(dll);

            ContextValidator.Validate(context, new AssemblyValidator());
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
            context.Assemblies.Add(file);

            ContextValidator.Validate(context, new AssemblyValidator());
            Assert.AreEqual(2, context.Assemblies.Count);
            Assert.Contains(dll1, context.Assemblies);
            Assert.Contains(dll2, context.Assemblies);
        }

        [Test]
        public void ValidFileAndAssemblyForAssemblyTest()
        {
            var dll1 = "dummy/path/to.dll";
            var dll2 = "dummy/path/to2.dll";
            var dll3 = "dummy/path/to3.dll";
            var dll4 = "dummy/path/to2.dll";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.Exists(dll1)).Returns(true);
            fileMock.Setup(x => x.Exists(dll2)).Returns(true);
            fileMock.Setup(x => x.Exists(dll3)).Returns(true);
            fileMock.Setup(x => x.Exists(dll4)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { dll1, dll2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Assemblies.Add(file);
            context.Assemblies.Add(dll3);
            context.Assemblies.Add(dll4);

            ContextValidator.Validate(context, new AssemblyValidator());
            Assert.AreEqual(3, context.Assemblies.Count);
            Assert.Contains(dll1, context.Assemblies);
            Assert.Contains(dll2, context.Assemblies);
            Assert.Contains(dll3, context.Assemblies);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ValidationException), ExpectedMessage = "Assembly is not a valid .dll file: 'dummy/path/to.txt'")]
        public void ValidFileWithInvalidDllForAssemblyTest()
        {
            var dll = "dummy/path/to.txt";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { dll });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Assemblies.Add(file);

            ContextValidator.Validate(context, new AssemblyValidator());
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ValidationException), ExpectedMessage = "File for list of assemblies not found: 'dummy/path/to/file'")]
        public void InvalidFileForAssemblyTest()
        {
            var dll = "dummy/path/to.dll";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { dll });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Assemblies.Add(file);

            ContextValidator.Validate(context, new AssemblyValidator());
        }

        #endregion

        #region Solution

        [Test]
        public void ValidSlnForSolutionTest()
        {
            var sln = "dummy/path/to.sln";
            var dll = @"dummy\path\Edison.Framework\bin\Debug\Edison.Framework.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(true);
            fileMock.Setup(x => x.ReadAllText(sln, Encoding.UTF8)).Returns("Project(\"{ FAE04EC0 - 301F - 11D3 - BF4B - 00C04F79EFBC}\") = \"Edison.Framework\", \"Edison.Framework\\Edison.Framework.csproj\", \"{ D7081147 - 8C02 - 4400 - 9B30 - 59D0AEC9591B}\"");
            fileMock.Setup(x => x.Exists(dll)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Solution = sln;

            ContextValidator.Validate(context, new AssemblyValidator());
            Assert.AreEqual(1, context.Assemblies.Count);
            Assert.Contains(dll, context.Assemblies);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ValidationException), ExpectedMessage = "Solution is not a valid .sln file: 'dummy/path/to.txt'")]
        public void InvalidSlnForSolutionTest()
        {
            var sln = "dummy/path/to.txt";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(false);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Solution = sln;

            ContextValidator.Validate(context, new AssemblyValidator());
        }

        [Test]
        public void ValidSolutionAndAssemblyTest()
        {
            var sln = "dummy/path/to.sln";
            var dll1 = @"dummy\path\Edison.Framework\bin\Debug\Edison.Framework.dll";
            var dll2 = "dummy/path/to.dll";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(sln)).Returns(true);
            fileMock.Setup(x => x.ReadAllText(sln, Encoding.UTF8)).Returns("Project(\"{ FAE04EC0 - 301F - 11D3 - BF4B - 00C04F79EFBC}\") = \"Edison.Framework\", \"Edison.Framework\\Edison.Framework.csproj\", \"{ D7081147 - 8C02 - 4400 - 9B30 - 59D0AEC9591B}\"");
            fileMock.Setup(x => x.Exists(dll1)).Returns(true);
            fileMock.Setup(x => x.Exists(dll2)).Returns(true);
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Assemblies.Add(dll2);
            context.Solution = sln;

            ContextValidator.Validate(context, new AssemblyValidator());
            Assert.AreEqual(2, context.Assemblies.Count);
            Assert.Contains(dll1, context.Assemblies);
            Assert.Contains(dll2, context.Assemblies);
        }

        #endregion

        #region Fixtures

        [Test]
        public void ValidFixtureTest()
        {
            var fixture = "this.is.some.fixture";
            
            var context = EdisonContext.Create();
            context.Fixtures.Add(fixture);

            ContextValidator.Validate(context, new NamespaceValidator());
        }

        [Test]
        public void ValidFileForFixturesTest()
        {
            var fixture1 = "this.is.some.fixture";
            var fixture2 = "this.is.some.fixture2";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { fixture1, fixture2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Fixtures.Add(file);

            ContextValidator.Validate(context, new NamespaceValidator());
            Assert.AreEqual(2, context.Fixtures.Count);
            Assert.Contains(fixture1, context.Fixtures);
            Assert.Contains(fixture2, context.Fixtures);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ValidationException), ExpectedMessage = "File for list of fixtures not found: 'dummy/path/to/file'")]
        public void InvalidFileForFixturesTest()
        {
            var fixture = "this.is.some.fixture";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { fixture });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Fixtures.Add(file);

            ContextValidator.Validate(context, new NamespaceValidator());
        }

        #endregion

        #region Tests

        [Test]
        public void ValidTestTest()
        {
            var test = "this.is.some.test";
            
            var context = EdisonContext.Create();
            context.Tests.Add(test);

            ContextValidator.Validate(context, new NamespaceValidator());
        }

        [Test]
        public void ValidFileForTestsTest()
        {
            var test1 = "this.is.some.test";
            var test2 = "this.is.some.test2";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(true);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { test1, test2 });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Tests.Add(file);

            ContextValidator.Validate(context, new NamespaceValidator());
            Assert.AreEqual(2, context.Tests.Count);
            Assert.Contains(test1, context.Tests);
            Assert.Contains(test2, context.Tests);
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(ValidationException), ExpectedMessage = "File for list of tests not found: 'dummy/path/to/file'")]
        public void InvalidFileForTestsTest()
        {
            var test = "this.is.some.test";
            var file = "dummy/path/to/file";

            var fileMock = new Mock<IFileRepository>();
            fileMock.Setup(x => x.Exists(file)).Returns(false);
            fileMock.Setup(x => x.ReadAllLines(file, Encoding.UTF8)).Returns(new string[] { test });
            DIContainer.Instance.BindAndCacheInstance<IFileRepository>(fileMock.Object);

            var context = EdisonContext.Create();
            context.Tests.Add(file);

            ContextValidator.Validate(context, new NamespaceValidator());
        }

        #endregion

    }
}
