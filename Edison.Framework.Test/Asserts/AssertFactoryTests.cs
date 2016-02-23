/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using EAssert = Edison.Framework.Assert;
using NAssert = NUnit.Framework.Assert;
using Edison.Framework.Enums;

namespace Edison.Framework.Test.Asserts
{
    [NUnit.Framework.TestFixture]
    public class AssertFactoryTests
    {

        [NUnit.Framework.Test]
        public void DefaultAssertFactoryTest()
        {
            AssertFactory.Instance = new EAssert();
            var assert = AssertFactory.Instance;
            NAssert.IsInstanceOf(typeof(IAssert), assert);

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
        public void CustoNAssertFactoryTest()
        {
            AssertFactory.Instance = new CustomAssert();
            var assert = AssertFactory.Instance;
            NAssert.IsInstanceOf(typeof(IAssert), assert);

            try
            {
                assert.AreEqual(5, 5);
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
