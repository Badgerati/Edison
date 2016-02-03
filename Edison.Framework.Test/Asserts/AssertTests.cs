/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using EAssert = Edison.Framework.Assert;
using MAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Edison.Framework.Tests
{
    [TestClass()]
    public class AssertTests
    {

        private IAssert GetAssert()
        {
            var assert = new EAssert();
            MAssert.IsInstanceOfType(assert, typeof(IAssert));
            return assert;
        }

        [TestMethod()]
        public void InconclusiveTest()
        {
            var assert = GetAssert();

            try
            {
                assert.Inconclusive();
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Inconclusive, aex.TestResultState);
                MAssert.AreEqual("Test marked as inconclusive", aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void FailTest()
        {
            var assert = GetAssert();

            try
            {
                assert.Fail();
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.AreEqual("Test marked as failed", aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void PassTest()
        {
            var assert = GetAssert();

            try
            {
                assert.Pass();
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Success, aex.TestResultState);
                MAssert.AreEqual("Test marked as passed", aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void AreEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.AreEqual(5, 5);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void AreNotEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.AreNotEqual(5, 7);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsInstanceOfTest()
        {
            var assert = GetAssert();

            try
            {
                var value = "test string";
                assert.IsInstanceOf<IComparable>(value);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotInstanceOfTest()
        {
            var assert = GetAssert();

            try
            {
                var value = "test string";
                assert.IsNotInstanceOf<int>(value);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsGreaterThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsGreaterThan(10, 6);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsGreaterThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsGreaterThanOrEqual(5, 5);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotGreaterThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotGreaterThan(10, 10);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotGreaterThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotGreaterThanOrEqual(10, 9);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsLessThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsLessThan(4, 5);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsLessThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsLessThanOrEqual(5, 5);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotLessThanTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotLessThan(5, 5);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotLessThanOrEqualTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotLessThanOrEqual(6, 5);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void FileExistsTest()
        {
            var assert = GetAssert();

            try
            {
                assert.FileExists("./Edison.Framework.dll");
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void FileDoesNotExistTest()
        {
            var assert = GetAssert();

            try
            {
                assert.FileDoesNotExist("./Edison.Me.No.Exist.dll");
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void DirectoryExistsTest()
        {
            var assert = GetAssert();

            try
            {
                assert.DirectoryExists("../Debug");
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void DirectoryDoesNotExistsTest()
        {
            var assert = GetAssert();

            try
            {
                assert.DirectoryDoesNotExists("../SomeFolder");
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNullTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNull(null);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotNullTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotNull("not null");
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsInstanceOfTypeTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsInstanceOfType(assert, typeof(IAssert));
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotInstanceOfTypeTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotInstanceOfType(assert, typeof(int));
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsZeroTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsZero(0);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotZeroTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotZero(1);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
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
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsBetweenTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsBetween(8, 7, 9);
            }
            catch (AssertException aex)
            {
                MAssert.Fail("Assert exception should not be thrown: " + aex.Message);
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

        [TestMethod()]
        public void IsNotBetweenTest()
        {
            var assert = GetAssert();

            try
            {
                assert.IsNotBetween(8, 6, 7);
            }
            catch (AssertException aex)
            {
                MAssert.IsNotNull(aex);
                MAssert.AreEqual(TestResultState.Failure, aex.TestResultState);
                MAssert.IsFalse(string.IsNullOrEmpty(aex.Message));
            }
            catch (Exception)
            {
                MAssert.Fail("Incorrect exception type");
            }
        }

    }
}