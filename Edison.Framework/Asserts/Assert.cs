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

        public virtual void Inconclusive(string message = null)
        {
            throw new AssertException(
                string.IsNullOrEmpty(message)
                    ? "Test marked as inconclusive"
                    : message,
                TestResultState.Inconclusive);
        }

        public virtual void Fail(string message = null)
        {
            throw new AssertException(
                string.IsNullOrEmpty(message)
                    ? "Test marked as failed"
                    : message,
                TestResultState.Failure);
        }

        public virtual void Pass(string message = null)
        {
            throw new AssertException(
                string.IsNullOrEmpty(message)
                    ? "Test marked as passed"
                    : message,
                TestResultState.Success);
        }

        #endregion

        #region Equals

        public virtual void AreEqual(IComparable expected, IComparable actual, string message = null)
        {
            if (!Equals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
            }
        }

        public virtual void AreNotEqual(IComparable expected, IComparable actual, string message = null)
        {
            if (Equals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }
        }

        public virtual void AreEnumerablesEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null) where T : IComparable
        {
            if (expected == default(IEnumerable<T>) && actual == default(IEnumerable<T>))
            {
                return;
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
        }

        public virtual void AreEnumerablesNotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null) where T : IComparable
        {
            if (expected == default(IEnumerable<T>) && actual == default(IEnumerable<T>))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }

            if ((expected == default(IEnumerable<T>) && actual != default(IEnumerable<T>))
                || (expected != default(IEnumerable<T>) && actual == default(IEnumerable<T>))
                || (expected.Count() != actual.Count()))
            {
                return;
            }

            var _expected = expected.ToList();
            var _actual = actual.ToList();

            for (var i = 0; i < _expected.Count; i++)
            {
                if (!Equals(_expected[i], _actual[i]))
                {
                    return;
                }
            }

            throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
        }

        public virtual void AreEqualIgnoreCase(string expected, string actual, string message = null)
        {
            if (!string.Equals(expected, actual, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
            }
        }

        public virtual void AreNotEqualIgnoreCase(string expected, string actual, string message = null)
        {
            if (string.Equals(expected, actual, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }
        }

        #endregion

        #region Same

        public virtual void AreSameReference(object expected, object actual, string message = null)
        {
            if (!ReferenceEquals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, null, expected, null, null, actual, null));
            }
        }

        public virtual void AreNotSameReference(object expected, object actual, string message = null)
        {
            if (ReferenceEquals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", expected, null, null, actual, null));
            }
        }

        #endregion

        #region Instance

        public virtual void IsInstanceOf<T>(object value, string message = null)
        {
            if (value == default(object))
            {
                throw new AssertException(ExpectedActualMessage(message, null, typeof(T).Name, null, null, null, null));
            }

            if (!(value is T))
            {
                throw new AssertException(ExpectedActualMessage(message, null, typeof(T).Name, null, null, value.GetType().Name, null));
            }
        }

        public virtual void IsNotInstanceOf<T>(object value, string message = null)
        {
            if (value != default(object) && (value is T))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", typeof(T).Name, null, null, value.GetType().Name, null));
            }
        }

        #endregion

        #region Boolean

        public virtual void IsTrue(bool value, string message = null)
        {
            if (!value)
            {
                throw new AssertException(ExpectedActualMessage(message, null, true, null, null, value, null));
            }
        }

        public virtual void IsFalse(bool value, string message = null)
        {
            if (value)
            {
                throw new AssertException(ExpectedActualMessage(message, null, false, null, null, value, null));
            }
        }

        #endregion

        #region Greater Than

        public virtual void IsGreaterThan(IComparable value, IComparable greaterThanThis, string message = null)
        {
            if (value == null || value.CompareTo(greaterThanThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than ", greaterThanThis, null, null, value, null));
            }
        }

        public virtual void IsGreaterThanOrEqual(IComparable value, IComparable greaterThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(greaterThanOrEqualToThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than or equal to ", greaterThanOrEqualToThis, null, null, value, null));
            }
        }

        public virtual void IsNotGreaterThan(IComparable value, IComparable notGreaterThanThis, string message = null)
        {
            if (value == null || value.CompareTo(notGreaterThanThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than or equal to ", notGreaterThanThis, null, null, value, null));
            }
        }

        public virtual void IsNotGreaterThanOrEqual(IComparable value, IComparable notGreaterThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(notGreaterThanOrEqualToThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than ", notGreaterThanOrEqualToThis, null, null, value, null));
            }
        }

        #endregion

        #region Less Than

        public virtual void IsLessThan(IComparable value, IComparable lessThanThis, string message = null)
        {
            if (value == null || value.CompareTo(lessThanThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than ", lessThanThis, null, null, value, null));
            }
        }

        public virtual void IsLessThanOrEqual(IComparable value, IComparable lessThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(lessThanOrEqualToThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Less than or equal to ", lessThanOrEqualToThis, null, null, value, null));
            }
        }

        public virtual void IsNotLessThan(IComparable value, IComparable notLessThanThis, string message = null)
        {
            if (value == null || value.CompareTo(notLessThanThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than or equal to ", notLessThanThis, null, null, value, null));
            }
        }

        public virtual void IsNotLessThanOrEqual(IComparable value, IComparable notLessThanOrEqualToThis, string message = null)
        {
            if (value == null || value.CompareTo(notLessThanOrEqualToThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Greater than ", notLessThanOrEqualToThis, null, null, value, null));
            }
        }

        #endregion

        #region Files and Directories

        public virtual void FileExists(string path, string message = null)
        {
            if (!File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File exists: ", path, null, null, "File does not exist", null));
            }
        }

        public virtual void FileDoesNotExist(string path, string message = null)
        {
            if (File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File does not exist: ", path, null, null, "File exists", null));
            }
        }

        public virtual void DirectoryExists(string path, string message = null)
        {
            if (!Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory exists: ", path, null, null, "Directory does not exist", null));
            }
        }

        public virtual void DirectoryDoesNotExists(string path, string message = null)
        {
            if (Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory does not exists: ", path, null, null, "Directory exists", null));
            }
        }

        #endregion

        #region Null, Default and Types

        public virtual void IsNull(object value, string message = null)
        {
            if (value != null)
            {
                throw new AssertException(ExpectedActualMessage(message, null, null, null, null, value, null));
            }
        }

        public virtual void IsNotNull(object value, string message = null)
        {
            if (value == null)
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", null, null, null, null, null));
            }
        }

        public virtual void IsDefault<T>(T value, string message = null)
        {
            if (!Equals(value, default(T)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Default ", typeof(T).Name, null, "Not default ", value.GetType().Name, null));
            }
        }

        public virtual void IsNotDefault<T>(T value, string message = null)
        {
            if (Equals(value, default(T)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not default ", typeof(T).Name, null, "Default ", typeof(T).Name, null));
            }
        }

        public virtual void IsInstanceOfType(object value, Type type, string message = null)
        {
            if (!type.IsInstanceOfType(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Instance of type ", type.Name, null, "Not instance of type ", value.GetType().Name, null));
            }
        }

        public virtual void IsNotInstanceOfType(object value, Type type, string message = null)
        {
            if (type.IsInstanceOfType(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not instance of type ", type.Name, null, "Instance of type ", value.GetType().Name, null));
            }
        }

        #endregion

        #region Zero

        public virtual void IsZero(IComparable value, string message = null)
        {
            if (!Equals(0, value))
            {
                throw new AssertException(ExpectedActualMessage(message, null, 0, null, null, value, null));
            }
        }

        public virtual void IsNotZero(IComparable value, string message = null)
        {
            if (Equals(0, value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not ", 0, null, null, value, null));
            }
        }

        #endregion

        #region Date and Time

        public virtual void AreDatesEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null)
        {
            if (minuteOffset != 0)
            {
                IsBetween(actual, expected.AddMinutes(-minuteOffset), expected.AddMinutes(minuteOffset), message);
            }
            else
            {
                AreEqual(expected, actual, message);
            }
        }

        public virtual void AreDatesNotEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null)
        {
            if (minuteOffset != 0)
            {
                IsNotBetween(actual, expected.AddMinutes(-minuteOffset), expected.AddMinutes(minuteOffset), message);
            }
            else
            {
                AreNotEqual(expected, actual, message);
            }
        }

        public virtual void AreTimesEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null)
        {
            if (offset != default(TimeSpan))
            {
                IsBetween(actual, expected.Subtract(offset), expected.Add(offset), message);
            }
            else
            {
                AreEqual(expected, actual, message);
            }
        }

        public virtual void AreTimesNotEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null)
        {
            if (offset != default(TimeSpan))
            {
                IsNotBetween(actual, expected.Subtract(offset), expected.Add(offset), message);
            }
            else
            {
                AreNotEqual(expected, actual, message);
            }
        }

        #endregion

        #region Between

        public virtual void IsBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null)
        {
            if (value == null || value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Between ", Safeguard(lowerBound, "NULL") + " and " + Safeguard(upperBound, "NULL"), null, null, value, null));
            }
        }

        public virtual void IsNotBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null)
        {
            if (value != null && (value.CompareTo(lowerBound) >= 0 && value.CompareTo(upperBound) <= 0))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not between ", Safeguard(lowerBound, "NULL") + " and " + Safeguard(upperBound, "NULL"), null, null, value, null));
            }
        }

        #endregion

        #region Contains

        public virtual void DoesContain(string value, string containsThis, string message = null)
        {
            if (value == null || !value.Contains(containsThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Contains '", containsThis, "'", null, value, null));
            }
        }

        public virtual void DoesNotContain(string value, string doesNotContainThis, string message = null)
        {
            if (value != null && value.Contains(doesNotContainThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not contain '", doesNotContainThis, "'", null, value, null));
            }
        }

        public virtual void DoesEnumerableContain<T>(IEnumerable<T> items, T containsThisItem, string message = null)
        {
            if (items == default(IEnumerable<T>) || !items.Contains(containsThisItem))
            {
                throw new AssertException(ExpectedActualMessage(message, "Contains ", containsThisItem, null, null, items, null));
            }
        }

        public virtual void DoesEnumerableNotContain<T>(IEnumerable<T> items, T doesNotContainThisItem, string message = null)
        {
            if (items != null && items.Contains(doesNotContainThisItem))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not contain ", doesNotContainThisItem, null, null, items, null));
            }
        }

        #endregion

        #region Matches

        public virtual void IsMatch(string pattern, string value, string message = null)
        {
            if (value == null || pattern == null || !Regex.IsMatch(value, pattern))
            {
                throw new AssertException(ExpectedActualMessage(message, "Matches '", pattern, "'", null, value, null));
            }
        }

        public virtual void IsNotMatch(string pattern, string value, string message = null)
        {
            if (value != null && (pattern == null || Regex.IsMatch(value, pattern)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not match '", pattern, "'", null, value, null));
            }
        }

        #endregion

        #region Starts and Ends With

        public virtual void StartsWith(string value, string startsWithThis, string message = null)
        {
            if (value == null || !value.StartsWith(startsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Starts with '", startsWithThis, "'", null, value, null));
            }
        }

        public virtual void DoesNotStartWith(string value, string doesNotStartsWithThis, string message = null)
        {
            if (value != null && value.StartsWith(doesNotStartsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not start with '", doesNotStartsWithThis, "'", null, value, null));
            }
        }

        public virtual void EndsWith(string value, string endsWithThis, string message = null)
        {
            if (value == null || !value.EndsWith(endsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "End with '", endsWithThis, "'", null, value, null));
            }
        }

        public virtual void DoesNotEndWith(string value, string doesNotEndsWithThis, string message = null)
        {
            if (value != null && value.EndsWith(doesNotEndsWithThis))
            {
                throw new AssertException(ExpectedActualMessage(message, "Does not end with '", doesNotEndsWithThis, "'", null, value, null));
            }
        }

        #endregion

        #region Empty

        public virtual void IsEmpty(string value, string message = null)
        {
            if (!Equals(value, string.Empty))
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Empty string", null, null, value, null));
            }
        }

        public virtual void IsNotEmpty(string value, string message = null)
        {
            if (Equals(value, string.Empty))
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Not an empty string", null, null, "Empty string", null));
            }
        }

        public virtual void IsEnumerableEmpty<T>(IEnumerable<T> value, string message = null)
        {
            if (value != default(IEnumerable<T>) && value.Any())
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Empty enumerable", null, null, value, null));
            }
        }

        public virtual void IsEnumerableNotEmpty<T>(IEnumerable<T> value, string message = null)
        {
            if (value == default(IEnumerable<T>) || !value.Any())
            {
                throw new AssertException(ExpectedActualMessage(message, null, "Not an empty enumerable", null, null, "Empty enumerable", null));
            }
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
                string.IsNullOrEmpty(premessage) ? "Test assertion failed" : premessage,
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
