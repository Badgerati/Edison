/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EAssert = Edison.Framework.Assert;
using MAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Edison.Framework.Enums;

namespace Edison.Framework.Test.Asserts
{
    [TestClass]
    public class AssertFactoryTests
    {

        [TestMethod]
        public void DefaultAssertFactoryTest()
        {
            AssertFactory.Instance = new Assert();
            var assert = AssertFactory.Instance;
            MAssert.IsInstanceOfType(assert, typeof(IAssert));

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

        [TestMethod]
        public void CustomAssertFactoryTest()
        {
            AssertFactory.Instance = new CustomAssert();
            var assert = AssertFactory.Instance;
            MAssert.IsInstanceOfType(assert, typeof(IAssert));

            try
            {
                assert.AreEqual(5, 5);
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


        private class CustomAssert : EAssert
        {
            public override void AreEqual(IComparable expected, IComparable actual, string message = null)
            {
                if (expected.Equals(actual))
                {
                    throw new AssertException(ExpectedActualMessage(message, expected, actual));
                }
            }
        }

    }
}
