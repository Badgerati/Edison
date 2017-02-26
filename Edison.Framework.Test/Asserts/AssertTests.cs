/*
Edison is designed to be simpler and more performant unit/integration NUnit.Framework.Testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Moq;
using System;
using EAssert = Edison.Framework.Assert;
using NAssert = NUnit.Framework.Assert;

namespace Edison.Framework.Tests
{
    [NUnit.Framework.TestFixture]
    public class AssertTests
    {

        private IAssert GetAssert()
        {
            var assert = new EAssert();
            NAssert.IsInstanceOf(typeof(IAssert), assert);
            return assert;
        }

        [NUnit.Framework.Test]
        public void InconclusiveTest()
        {
            var assert = GetAssert();

            try
            {
                assert.Inconclusive();
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Inconclusive, aex.TestResultState);
                NAssert.AreEqual("Test marked as inconclusive", aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void FailTest()
        {
            var assert = GetAssert();

            try
            {
                assert.Fail();
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.AreEqual("Test marked as failed", aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void PassTest()
        {
            var assert = GetAssert();

            try
            {
                assert.Pass();
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Success, aex.TestResultState);
                NAssert.AreEqual("Test marked as passed", aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void Or_AllPass_Success_Test()
        {
            var assert = GetAssert();

            try
            {
                assert.Or(() => assert.AreEqual(5, 5),
                          () => assert.AreEqual(1, 1));
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void Or_OnePass_Success_Test()
        {
            var assert = GetAssert();

            try
            {
                assert.Or(() => assert.AreEqual(5, 6),
                          () => assert.AreEqual(1, 1));
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void Or_NonePass_Fail_Test()
        {
            var assert = GetAssert();

            try
            {
                assert.Or(() => assert.AreEqual(5, 6),
                          () => assert.AreEqual(1, 2));
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.AreEqual(5, 5);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreNotEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.AreNotEqual(5, 7);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreEnumerablesEqualTest()
        {
            var assert = GetAssert();

            try
            {
                var values1 = new[] { 1, 3, 7, 4 };
                var values2 = new[] { 1, 3, 7, 4 };
                assert.AreEnumerablesEqual(values1, values2);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreEnumerablesNotEqualTest()
        {
            var assert = GetAssert();

            try
            {
                var values1 = new[] { 1, 3, 7, 4 };
                var values2 = new[] { 1, 3, 7, 5 };
                assert.AreEnumerablesNotEqual(values1, values2);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreEqualIgnoreCaseTest()
        {
            var assert = GetAssert();

            try
            {
                assert.AreEqualIgnoreCase("Hello", "hELLO");
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreNotEqualIgnoreCaseTest()
        {
            var assert = GetAssert();

            try
            {
                assert.AreNotEqualIgnoreCase("Hello, World", "hELLo wORLd");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void ContainsTest()
        {
            var assert = GetAssert();

            try
            {
                assert.DoesContain("This is a test", "a tes");
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void DoesNotContainTest()
        {
            var assert = GetAssert();

            try
            {
                assert.DoesNotContain("This is a test", "hello");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void ListContainsTest()
        {
            var assert = GetAssert();

            try
            {
                var values = new[] { 1, 2, 3, 4, 5 };
                assert.DoesEnumerableContain(values, 3);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void ListDoesNotContainTest()
        {
            var assert = GetAssert();

            try
            {
                var values = new[] { 1, 2, 3, 4, 5 };
                assert.DoesEnumerableNotContain(values, 7);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsMatchTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsMatch(@"^\d+$", "13579");
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotMatchTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotMatch(@"^\d+$", "13a79");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void StartsWithTest()
        {
            var assert = GetAssert();

            try
            {
                assert.StartsWith("Hello, world", "Hello");
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void DoesNotStartWithTest()
        {
            var assert = GetAssert();

            try
            {
                assert.DoesNotStartWith("Hello, world", "Bob");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void EndsWithTest()
        {
            var assert = GetAssert();

            try
            {
                assert.EndsWith("Hello, world", "world");
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void DoesNotEndWithTest()
        {
            var assert = GetAssert();

            try
            {
                assert.DoesNotEndWith("Hello, world", "Bob");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsEmptyTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsEmpty(string.Empty);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotEmptyTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotEmpty("Test");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsEnumerableEmptyTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsEnumerableEmpty(new int[] { });
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsEnumerbaleNotEmptyTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsEnumerableNotEmpty(new int[] { 1 });
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreSameReferenceTest()
        {
            var assert = GetAssert();

            try
            {
                var value1 = "Hello!";
                var value2 = "Hello!";
                value1 = value2;
                assert.AreSameReference(value1, value2);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreNotSameReferenceTest()
        {
            var assert = GetAssert();

            try
            {
                var value1 = "Hello!";
                var value2 = "Heo";
                assert.AreNotSameReference(value1, value2);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsInstanceOfTest()
        {
            var assert = GetAssert();

            try
            {
                var value = "NUnit.Framework.Test string";
                assert.IsInstanceOf<IComparable>(value);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotInstanceOfTest()
        {
            var assert = GetAssert();

            try
            {
                var value = "NUnit.Framework.Test string";
                assert.IsNotInstanceOf<int>(value);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsTrueTest()
        {
            var assert = GetAssert();

            try
            {
                var value = true;
                assert.IsTrue(value);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsFalseTest()
        {
            var assert = GetAssert();

            try
            {
                var value = false;
                assert.IsFalse(value);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsGreaterThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsGreaterThan(10, 6);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsGreaterThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsGreaterThanOrEqual(5, 5);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotGreaterThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotGreaterThan(10, 10);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotGreaterThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotGreaterThanOrEqual(10, 9);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsLessThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsLessThan(4, 5);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsLessThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsLessThanOrEqual(5, 5);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotLessThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotLessThan(5, 5);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotLessThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotLessThanOrEqual(6, 5);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void FileExistsTest()
        {
            var assert = GetAssert();

            try
            {
                assert.FileExists("./Edison.Framework.dll");
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void FileDoesNotExistTest()
        {
            var assert = GetAssert();

            try
            {
                assert.FileDoesNotExist("./Edison.Me.No.Exist.dll");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void DirectoryExistsTest()
        {
            var assertMock = new Mock<IAssert>();
            assertMock.Setup(x => x.DirectoryExists("C:/Users", string.Empty));

            try
            {
                assertMock.Object.DirectoryExists("C:/Users");
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void DirectoryDoesNotExistsTest()
        {
            var assert = GetAssert();

            try
            {
                assert.DirectoryDoesNotExists("../SomeFolder");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNullTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNull(null);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotNullTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotNull("not null");
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsDefaultTest()
        {
            var assert = GetAssert();

            try
            {
                var value = default(int);
                assert.IsDefault<int>(value);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotDefaultTest()
        {
            var assert = GetAssert();

            try
            {
                var value = 128;
                assert.IsNotDefault<int>(value);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsInstanceOfTypeTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsInstanceOfType(assert, typeof(IAssert));
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotInstanceOfTypeTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotInstanceOfType(assert, typeof(int));
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsZeroTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsZero(0);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotZeroTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotZero(1);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreDatesEqualTest()
        {
            var assert = GetAssert();

            try
            {
                var now = DateTime.Now;
                assert.AreDatesEqual(now, now);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreDatesNotEqualTest()
        {
            var assert = GetAssert();

            try
            {
                var now = DateTime.Now;
                assert.AreDatesNotEqual(now, now.AddDays(1));
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreTimesEqualTest()
        {
            var assert = GetAssert();

            try
            {
                var now = DateTime.Now.TimeOfDay;
                assert.AreTimesEqual(now, now);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void AreTimesNotEqualTest()
        {
            var assert = GetAssert();

            try
            {
                var now = DateTime.Now.TimeOfDay;
                assert.AreTimesNotEqual(now, now.Add(TimeSpan.FromMinutes(5)));
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsBetweenTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsBetween(8, 7, 9);
            }
            catch (AssertException aex)
            {
                NAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

        [NUnit.Framework.Test]
        public void IsNotBetweenTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotBetween(8, 6, 7);
            }
            catch (AssertException aex)
            {
                NAssert.IsNotNull(aex);
                NAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                NAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                NAssert.Fail("Incorrect exception type");
            }
        }

    }
}