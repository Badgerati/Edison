/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;

namespace Edison.Framework
{
    public interface IAssert
    {

        IAssert Inconclusive(string message = null);
        IAssert Fail(string message = null);
        IAssert Pass(string message = null);

        IAssert AreEqual(IComparable expected, IComparable actual, string message = null);
        IAssert AreNotEqual(IComparable expected, IComparable actual, string message = null);

        IAssert AreEnumerablesEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null) where T : IComparable;
        IAssert AreEnumerablesNotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null) where T : IComparable;

        IAssert AreEqualIgnoreCase(string expected, string actual, string message = null);
        IAssert AreNotEqualIgnoreCase(string expected, string actual, string message = null);

        IAssert DoesEnumerableContain<T>(IEnumerable<T> items, T containsThisItem, string message = null);
        IAssert DoesEnumerableNotContain<T>(IEnumerable<T> items, T doesNotContainThisItem, string message = null);

        IAssert DoesContain(string value, string containsThis, string message = null);
        IAssert DoesNotContain(string value, string doesNotContainThis, string message = null);

        IAssert IsMatch(string pattern, string value, string message = null);
        IAssert IsNotMatch(string pattern, string value, string message = null);

        IAssert StartsWith(string value, string startsWithThis, string message = null);
        IAssert DoesNotStartWith(string value, string doeNotStartWithThis, string message = null);

        IAssert EndsWith(string value, string endsWithThis, string message = null);
        IAssert DoesNotEndWith(string value, string doesNotEndWithThis, string message = null);

        IAssert AreSameReference(object expected, object actual, string message = null);
        IAssert AreNotSameReference(object expected, object actual, string message = null);

        IAssert IsInstanceOf<T>(object value, string message = null);
        IAssert IsNotInstanceOf<T>(object value, string message = null);

        IAssert IsTrue(bool value, string message = null);
        IAssert IsFalse(bool value, string message = null);

        IAssert IsGreaterThan(IComparable value, IComparable greaterThanThis, string message = null);
        IAssert IsGreaterThanOrEqual(IComparable value, IComparable greaterThanOrEqualToThis, string message = null);
        IAssert IsNotGreaterThan(IComparable value, IComparable notGreaterThanThis, string message = null);
        IAssert IsNotGreaterThanOrEqual(IComparable value, IComparable notGreaterThanOrEqualToThis, string message = null);

        IAssert IsLessThan(IComparable value, IComparable lessThanThis, string message = null);
        IAssert IsLessThanOrEqual(IComparable value, IComparable lessThanOrEqualToThis, string message = null);
        IAssert IsNotLessThan(IComparable value, IComparable notLessThanThis, string message = null);
        IAssert IsNotLessThanOrEqual(IComparable value, IComparable notLessThanOrEqualToThis, string message = null);

        IAssert FileExists(string path, string message = null);
        IAssert FileDoesNotExist(string path, string message = null);

        IAssert DirectoryExists(string path, string message = null);
        IAssert DirectoryDoesNotExists(string path, string message = null);

        IAssert IsNull(object value, string message = null);
        IAssert IsNotNull(object value, string message = null);

        IAssert IsDefault<T>(T value, string message = null);
        IAssert IsNotDefault<T>(T value, string message = null);

        IAssert IsInstanceOfType(object value, Type type, string message = null);
        IAssert IsNotInstanceOfType(object value, Type type, string message = null);

        IAssert IsZero(IComparable value, string message = null);
        IAssert IsNotZero(IComparable value, string message = null);

        IAssert AreDatesEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null);
        IAssert AreDatesNotEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null);

        IAssert AreTimesEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null);
        IAssert AreTimesNotEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null);

        IAssert IsBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null);
        IAssert IsNotBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null);

        IAssert IsEmpty(string value, string message = null);
        IAssert IsNotEmpty(string value, string message = null);

        IAssert IsEnumerableEmpty<T>(IEnumerable<T> value, string message = null);
        IAssert IsEnumerableNotEmpty<T>(IEnumerable<T> value, string message = null);

        IAssert Or(params Func<IAssert>[] asserts);

        IAssert ExpectUrl(IBrowser browser, string expectedUrl, int attempts = 10, bool startsWith = false, string message = null);
        IAssert ExpectElement(IBrowser browser, HtmlElementIdentifierType identifierType, string expectedIdentifier, int attempts = 10, string message = null);
        IAssert ExpectValue(IBrowser browser, string expectedValue, int attempts = 10, string message = null);

    }
}
