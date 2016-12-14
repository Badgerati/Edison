/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Edison.Framework
{
    public class Assert : IAssert
    {

        #region Test state

        public virtual IAssert Inconclusive(string message = null)
        {
            throw new AssertException(
                string.IsNullOrWhiteSpace(message)
                    ? "Test marked as inconclusive"
                    : message,
                TestResultState.Inconclusive);
        }

        public virtual IAssert Fail(string message = null)
        {
            throw new AssertException(
                string.IsNullOrWhiteSpace(message)
                    ? "Test marked as failed"
                    : message,
                TestResultState.Failure);
        }

        public virtual IAssert Pass(string message = null)
        {
            throw new AssertException(
                string.IsNullOrWhiteSpace(message)
                    ? "Test marked as passed"
                    : message,
                TestResultState.Success);
        }

        #endregion

        #region Equals

        public virtual IAssert AreEqual(IComparable expected, IComparable actual, string message = null)
        {
            if (!Equals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
            }

            return this;
        }

        public virtual IAssert AreNotEqual(IComparable expected, IComparable actual, string message = null)
        {
            if (Equals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }

            return this;
        }

        public virtual IAssert AreEnumerablesEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null) where T : IComparable
        {
            if (expected == default(IEnumerable<T>) && actual == default(IEnumerable<T>))
            {
                return this;
            }

            if (expected == default(IEnumerable<T>) || actual == default(IEnumerable<T>) || expected.Count() != actual.Count())
            {
                throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
            }

            var _expected = expected.ToList();
            var _actual = actual.ToList();

            for (var i = 0; i < _expected.Count; i++)
            {
                if (!Equals(_expected[i], _actual[i]))
                {
                    throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
                }
            }

            return this;
        }

        public virtual IAssert AreEnumerablesNotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null) where T : IComparable
        {
            if (expected == default(IEnumerable<T>) && actual == default(IEnumerable<T>))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }

            if ((expected == default(IEnumerable<T>) && actual != default(IEnumerable<T>))
                || (expected != default(IEnumerable<T>) && actual == default(IEnumerable<T>))
                || (expected.Count() != actual.Count()))
            {
                return this;
            }

            var _expected = expected.ToList();
            var _actual = actual.ToList();

            for (var i = 0; i < _expected.Count; i++)
            {
                if (!Equals(_expected[i], _actual[i]))
                {
                    return this;
                }
            }

            throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
        }

        public virtual IAssert AreEqualIgnoreCase(string expected, string actual, string message = null)
        {
            if (!string.Equals(expected, actual, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
            }

            return this;
        }

        public virtual IAssert AreNotEqualIgnoreCase(string expected, string actual, string message = null)
        {
            if (string.Equals(expected, actual, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }

            return this;
        }

        #endregion

        #region Same

        public virtual IAssert AreSameReference(object expected, object actual, string message = null)
        {
            if (!ReferenceEquals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
            }

            return this;
        }

        public virtual IAssert AreNotSameReference(object expected, object actual, string message = null)
        {
            if (ReferenceEquals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }

            return this;
        }

        #endregion

        #region Instance

        public virtual IAssert IsInstanceOf<T>(object value, string message = null)
        {
            if (value == default(object))
            {
                throw new AssertException(ExpectedActualMessage(message, null, typeof(T).Name, null, null, null, null));
            }

            if (!(value is T))
            {
                throw new AssertException(ExpectedActualMessage(message, null, typeof(T).Name, null, null, value.GetType().Name, null));
            }

            return this;
        }

        public virtual IAssert IsNotInstanceOf<T>(object value, string message = null)
        {
            if (value != default(object) && (value is T))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", typeof(T).Name, null, null, value.GetType().Name, null));
            }

            return this;
        }

        #endregion

        #region Boolean

        public virtual IAssert IsTrue(bool value, string message = null)
        {
            if (!value)
            {
                throw new AssertException(ExpectedActualMessage(message, null, true, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsFalse(bool value, string message = null)
        {
            if (value)
            {
                throw new AssertException(ExpectedActualMessage(message, null, false, null, null, value, null));
            }

            return this;
        }

        #endregion

        #region Greater Than

        public virtual IAssert IsGreaterThan(IComparable value, IComparable greaterThanThis, string message = null)
        {
            if (value == null || value.CompareTo(greaterThanThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than ", greaterThanThis, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsGreaterThanOrEqual(IComparable value, IComparable greaterThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(greaterThanOrEqualToThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than or equal to ", greaterThanOrEqualToThis, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotGreaterThan(IComparable value, IComparable notGreaterThanThis, string message = null)
        {
            if (value == null || value.CompareTo(notGreaterThanThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than or equal to ", notGreaterThanThis, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotGreaterThanOrEqual(IComparable value, IComparable notGreaterThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(notGreaterThanOrEqualToThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than ", notGreaterThanOrEqualToThis, null, null, value, null));
            }

            return this;
        }

        #endregion

        #region Less Than

        public virtual IAssert IsLessThan(IComparable value, IComparable lessThanThis, string message = null)
        {
            if (value == null || value.CompareTo(lessThanThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than ", lessThanThis, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsLessThanOrEqual(IComparable value, IComparable lessThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(lessThanOrEqualToThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than or equal to ", lessThanOrEqualToThis, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotLessThan(IComparable value, IComparable notLessThanThis, string message = null)
        {
            if (value == null || value.CompareTo(notLessThanThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than or equal to ", notLessThanThis, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotLessThanOrEqual(IComparable value, IComparable notLessThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(notLessThanOrEqualToThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than ", notLessThanOrEqualToThis, null, null, value, null));
            }

            return this;
        }

        #endregion

        #region Files

        public virtual IAssert FileExists(string path, string message = null)
        {
            if (!File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File exists: ", path, null, null, "File does not exist", null));
            }

            return this;
        }

        public virtual IAssert FileDoesNotExist(string path, string message = null)
        {
            if (File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File does not exist: ", path, null, null, "File exists", null));
            }

            return this;
        }

        #endregion

        #region Directories

        public virtual IAssert DirectoryExists(string path, string message = null)
        {
            if (!Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory exists: ", path, null, null, "Directory does not exist", null));
            }

            return this;
        }

        public virtual IAssert DirectoryDoesNotExists(string path, string message = null)
        {
            if (Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory does not exists: ", path, null, null, "Directory exists", null));
            }

            return this;
        }

        #endregion

        #region Null

        public virtual IAssert IsNull(object value, string message = null)
        {
            if (value != null)
            {
                throw new AssertException(ExpectedActualMessage(message, null, null, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotNull(object value, string message = null)
        {
            if (value == null)
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", null, null, null, null, null));
            }

            return this;
        }

        #endregion

        #region Defaults

        public virtual IAssert IsDefault<T>(T value, string message = null)
        {
            if (!Equals(value, default(T)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Default ", typeof(T).Name, null, "Not default ", value.GetType().Name, null));
            }

            return this;
        }

        public virtual IAssert IsNotDefault<T>(T value, string message = null)
        {
            if (Equals(value, default(T)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not default ", typeof(T).Name, null, "Default ", typeof(T).Name, null));
            }

            return this;
        }

        #endregion

        #region Types

        public virtual IAssert IsInstanceOfType(object value, Type type, string message = null)
        {
            if (!type.IsInstanceOfType(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Instance of type ", type.Name, null, "Not instance of type ", value.GetType().Name, null));
            }

            return this;
        }

        public virtual IAssert IsNotInstanceOfType(object value, Type type, string message = null)
        {
            if (type.IsInstanceOfType(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not instance of type ", type.Name, null, "Instance of type ", value.GetType().Name, null));
            }

            return this;
        }

        #endregion

        #region Zero

        public virtual IAssert IsZero(IComparable value, string message = null)
        {
            if (!Equals(0, value))
            {
                throw new AssertException(ExpectedActualMessage(message, null, 0, null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotZero(IComparable value, string message = null)
        {
            if (Equals(0, value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", 0, null, null, value, null));
            }

            return this;
        }

        #endregion

        #region Date

        public virtual IAssert AreDatesEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null)
        {
            if (minuteOffset != 0)
            {
                IsBetween(actual, expected.AddMinutes(-minuteOffset), expected.AddMinutes(minuteOffset), message);
            }
            else
            {
                AreEqual(expected, actual, message);
            }

            return this;
        }

        public virtual IAssert AreDatesNotEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null)
        {
            if (minuteOffset != 0)
            {
                IsNotBetween(actual, expected.AddMinutes(-minuteOffset), expected.AddMinutes(minuteOffset), message);
            }
            else
            {
                AreNotEqual(expected, actual, message);
            }

            return this;
        }

        #endregion

        #region Time

        public virtual IAssert AreTimesEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null)
        {
            if (offset != default(TimeSpan))
            {
                IsBetween(actual, expected.Subtract(offset), expected.Add(offset), message);
            }
            else
            {
                AreEqual(expected, actual, message);
            }

            return this;
        }

        public virtual IAssert AreTimesNotEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null)
        {
            if (offset != default(TimeSpan))
            {
                IsNotBetween(actual, expected.Subtract(offset), expected.Add(offset), message);
            }
            else
            {
                AreNotEqual(expected, actual, message);
            }

            return this;
        }

        #endregion

        #region Between

        public virtual IAssert IsBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null)
        {
            if (value == null || value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Between ", Safeguard(lowerBound, "NULL") + " and " + Safeguard(upperBound, "NULL"), null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null)
        {
            if (value != null && (value.CompareTo(lowerBound) >= 0 && value.CompareTo(upperBound) <= 0))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not between ", Safeguard(lowerBound, "NULL") + " and " + Safeguard(upperBound, "NULL"), null, null, value, null));
            }

            return this;
        }

        #endregion

        #region Contains

        public virtual IAssert DoesContain(string value, string containsThis, string message = null)
        {
            if (value == null || !value.Contains(containsThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Contains '", containsThis, "'", null, value, null));
            }

            return this;
        }

        public virtual IAssert DoesNotContain(string value, string doesNotContainThis, string message = null)
        {
            if (value != null && value.Contains(doesNotContainThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not contain '", doesNotContainThis, "'", null, value, null));
            }

            return this;
        }

        public virtual IAssert DoesEnumerableContain<T>(IEnumerable<T> items, T containsThisItem, string message = null)
        {
            if (items == default(IEnumerable<T>) || !items.Contains(containsThisItem))
            {
                throw new AssertException(ExpectedActualMessage(message, "Contains ", containsThisItem, null, null, items, null));
            }

            return this;
        }

        public virtual IAssert DoesEnumerableNotContain<T>(IEnumerable<T> items, T doesNotContainThisItem, string message = null)
        {
            if (items != null && items.Contains(doesNotContainThisItem))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not contain ", doesNotContainThisItem, null, null, items, null));
            }

            return this;
        }

        #endregion

        #region Matches

        public virtual IAssert IsMatch(string pattern, string value, string message = null)
        {
            if (value == null || pattern == null || !Regex.IsMatch(value, pattern))
            {
                throw new AssertException(ExpectedActualMessage(message, "Matches '", pattern, "'", null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotMatch(string pattern, string value, string message = null)
        {
            if (value != null && (pattern == null || Regex.IsMatch(value, pattern)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not match '", pattern, "'", null, value, null));
            }

            return this;
        }

        #endregion

        #region Starts With

        public virtual IAssert StartsWith(string value, string startsWithThis, string message = null)
        {
            if (value == null || !value.StartsWith(startsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Starts with '", startsWithThis, "'", null, value, null));
            }

            return this;
        }

        public virtual IAssert DoesNotStartWith(string value, string doesNotStartsWithThis, string message = null)
        {
            if (value != null && value.StartsWith(doesNotStartsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not start with '", doesNotStartsWithThis, "'", null, value, null));
            }

            return this;
        }

        #endregion

        #region Ends With

        public virtual IAssert EndsWith(string value, string endsWithThis, string message = null)
        {
            if (value == null || !value.EndsWith(endsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "End with '", endsWithThis, "'", null, value, null));
            }

            return this;
        }

        public virtual IAssert DoesNotEndWith(string value, string doesNotEndsWithThis, string message = null)
        {
            if (value != null && value.EndsWith(doesNotEndsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not end with '", doesNotEndsWithThis, "'", null, value, null));
            }

            return this;
        }

        #endregion

        #region Empty

        public virtual IAssert IsEmpty(string value, string message = null)
        {
            if (!Equals(value, string.Empty))
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Empty string", null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsNotEmpty(string value, string message = null)
        {
            if (Equals(value, string.Empty))
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Not an empty string", null, null, "Empty string", null));
            }

            return this;
        }

        public virtual IAssert IsEnumerableEmpty<T>(IEnumerable<T> value, string message = null)
        {
            if (value != default(IEnumerable<T>) && value.Any())
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Empty enumerable", null, null, value, null));
            }

            return this;
        }

        public virtual IAssert IsEnumerableNotEmpty<T>(IEnumerable<T> value, string message = null)
        {
            if (value == default(IEnumerable<T>) || !value.Any())
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Not an empty enumerable", null, null, "Empty enumerable", null));
            }

            return this;
        }

        #endregion

        #region Or

        public virtual IAssert Or(params Func<IAssert>[] asserts)
        {
            if (asserts == default(Func<IAssert>[]) || !asserts.Any())
            {
                return this;
            }

            var fails = new List<AssertException>(asserts.Length);

            foreach (var assert in asserts)
            {
                try
                {
                    assert();
                    return this;
                }
                catch (AssertException ex)
                {
                    fails.Add(ex);
                }
            }

            if (fails.Count == asserts.Length)
            {
                throw new AssertException(string.Join<string>(Environment.NewLine + Environment.NewLine, fails.Select(x => x.Message)));
            }

            return this;
        }

        #endregion

        #region Browser
        
        public virtual IAssert ExpectUrl(IBrowser browser, string expectedUrl, int attempts = 10, bool startsWith = false, string message = null)
        {
            var count = 0;

            while (!(startsWith && browser.URL.StartsWith(expectedUrl)) && !browser.URL.Equals(expectedUrl))
            {
                if (count >= attempts)
                {
                    throw new AssertException(ExpectedActualMessage(message, null, expectedUrl, null, null, browser.URL, null));
                }

                count++;
                browser.Sleep(1000);
            }

            return this;
        }

        public virtual IAssert ExpectElement(IBrowser browser, HtmlIdentifierType identifierType, string expectedIdentifier, int attempts = 10, string message = null)
        {
            var count = 0;
            var control = default(dynamic);

            while (control == null)
            {
                if (count >= attempts)
                {
                    throw new AssertException(ExpectedActualMessage(message, null, expectedIdentifier, null, "Element not found at: ", browser.URL, null));
                }

                control = browser.Get(identifierType, expectedIdentifier);

                count++;
                browser.Sleep(1000);
            }

            return this;
        }

        public virtual IAssert ExpectValue(IBrowser browser, string expectedValue, int attempts = 10, string message = null)
        {
            var count = 0;
            var regex = new Regex(expectedValue);

            while (!regex.IsMatch(browser.Body))
            {
                if (count >= attempts)
                {
                    throw new AssertException(ExpectedActualMessage(message, null, expectedValue, null, "Value not found at: ", browser.URL, null));
                }

                count++;
                browser.Sleep(1000);
            }

            return this;
        }

        #endregion

        #region Protected Helpers

        protected virtual string ExpectedActualMessage(string premessage, string preExpected, object expected, string postExpected, string preActual, object actual, string postActual)
        {
            if (expected != default(object) && !(expected is string) && (expected is IEnumerable))
            {
                expected = BuildEnumerableString((IEnumerable)expected);
            }

            if (actual != default(object) && !(actual is string) && (actual is IEnumerable))
            {
                actual = BuildEnumerableString((IEnumerable)actual);
            }

            return string.Format("{1}{0}Expected:\t{2}{0}But was:\t{3}",
                Environment.NewLine,
                string.IsNullOrWhiteSpace(premessage) ? "Test assertion failed" : premessage,
                Safeguard(preExpected) + Safeguard(expected, "NULL") + Safeguard(postExpected),
                Safeguard(preActual) + Safeguard(actual, "NULL") + Safeguard(postActual));
        }

        protected string Safeguard(object value, string defaultValue = "")
        {
            return value == default(object)
                ? defaultValue
                : value.ToString();
        }

        protected string BuildEnumerableString(IEnumerable items)
        {
            var builder = new StringBuilder();

            foreach (var item in items)
            {
                builder.Append(item + ", ");
            }

            return builder.ToString().Replace(Environment.NewLine, string.Empty).Trim('\n', ',', ' ', '\r');
        }

        #endregion

    }
}
