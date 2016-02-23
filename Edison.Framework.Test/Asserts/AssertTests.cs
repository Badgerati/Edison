/*
Edison is designed to be simpler and more performant unit/integration NUnit.Framework.Testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
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