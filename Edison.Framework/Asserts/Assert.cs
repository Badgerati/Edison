/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Framework
{
    public class Assert : IAssert
    {

        #region Test state

        public void Inconclusive(string message = null)
        {
            throw new AssertException(
                string.IsNullOrEmpty(message)
                    ? "Test marked as inconclusive"
                    : message,
                TestResultState.Inconclusive);
        }

        public void Fail(string message = null)
        {
            throw new AssertException(
                string.IsNullOrEmpty(message)
                    ? "Test marked as failed"
                    : message,
                TestResultState.Failure);
        }

        public void Pass(string message = null)
        {
            throw new AssertException(
                string.IsNullOrEmpty(message)
                    ? "Test marked as passed"
                    : message,
                TestResultState.Success);
        }

        #endregion

        #region Equals

        public void AreEqual(IComparable expected, IComparable actual, string message = null)
        {
            if (!expected.Equals(actual))
            {
                throw new AssertException(ExpectedActualMessage(message, expected, actual));
            }
        }

        public void AreNotEqual(IComparable expected, IComparable actual, string message = null)
        {
            if (expected.Equals(actual))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not " + expected, actual));
            }
        }

        #endregion

        #region Greater Than

        public void IsGreaterThan(IComparable value, IComparable greaterThanThis, string message = null)
        {
            if (value.CompareTo(greaterThanThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">" + greaterThanThis, value));
            }
        }

        public void IsGreaterThanOrEqual(IComparable value, IComparable greaterThanThis, string message = null)
        {
            if (value.CompareTo(greaterThanThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">=" + greaterThanThis, value));
            }
        }

        public void IsNotGreaterThan(IComparable value, IComparable notGreaterThanThis, string message = null)
        {
            if (value.CompareTo(notGreaterThanThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<=" + notGreaterThanThis, value));
            }
        }

        public void IsNotGreaterThanOrEqual(IComparable value, IComparable notGreaterThanThis, string message = null)
        {
            if (value.CompareTo(notGreaterThanThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<" + notGreaterThanThis, value));
            }
        }

        #endregion

        #region Less Than

        public void IsLessThan(IComparable value, IComparable lessThanThis, string message = null)
        {
            if (value.CompareTo(lessThanThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<" + lessThanThis, value));
            }
        }

        public void IsLessThanOrEqual(IComparable value, IComparable lessThanThis, string message = null)
        {
            if (value.CompareTo(lessThanThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<=" + lessThanThis, value));
            }
        }

        public void IsNotLessThan(IComparable value, IComparable notLessThanThis, string message = null)
        {
            if (value.CompareTo(notLessThanThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">=" + notLessThanThis, value));
            }
        }

        public void IsNotLessThanOrEqual(IComparable value, IComparable notLessThanThis, string message = null)
        {
            if (value.CompareTo(notLessThanThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">" + notLessThanThis, value));
            }
        }

        #endregion

        #region Files and Directories

        public void FileExists(string path, string message = null)
        {
            if (!File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File exists: " + path, "File does not exist"));
            }
        }

        public void FileDoesNotExist(string path, string message = null)
        {
            if (File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File does not exist: " + path, "File exists"));
            }
        }

        public void DirectoryExists(string path, string message = null)
        {
            if (!Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory exists: " + path, "Directory does not exist"));
            }
        }

        public void DirectoryDoesNotExists(string path, string message = null)
        {
            if (Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory does not exists: " + path, "Directory exists"));
            }
        }

        #endregion

        #region Null or Default

        public void IsNull(object value, string message = null)
        {
            if (value != null)
            {
                throw new AssertException(ExpectedActualMessage(message, "NULL", value));
            }
        }

        public void IsNotNull(object value, string message = null)
        {
            if (value == null)
            {
                throw new AssertException(ExpectedActualMessage(message, "Not NULL", "NULL"));
            }
        }

        public void IsDefault<T>(T value, string message = null)
        {
            if (!EqualityComparer<T>.Default.Equals(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Default " + typeof(T).Name, "Not default " + value.GetType().Name));
            }
        }

        public void IsNotDefault<T>(T value, string message = null)
        {
            if (EqualityComparer<T>.Default.Equals(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not default " + typeof(T).Name, "Default " + typeof(T).Name));
            }
        }

        #endregion

        #region Zero

        public void IsZero(IComparable value, string message = null)
        {
            if (!value.Equals(0))
            {
                throw new AssertException(ExpectedActualMessage(message, "0", value));
            }
        }

        public void IsNotZero(IComparable value, string message = null)
        {
            if (value.Equals(0))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not 0", value));
            }
        }

        #endregion

        #region Date and Time

        public void AreDatesEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null)
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

        public void AreDatesNotEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null)
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

        public void AreTimesEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null)
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

        public void AreTimesNotEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null)
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

        public void IsBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null)
        {
            if (value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Between " + lowerBound + " and " + upperBound, value));
            }
        }

        public void IsNotBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null)
        {
            if (value.CompareTo(lowerBound) >= 0 && value.CompareTo(upperBound) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Not between " + lowerBound + " and " + upperBound, value));
            }
        }

        #endregion

        #region Private Helpers

        private string ExpectedActualMessage(string premessage, object expected, object actual)
        {
            return string.Format("Error Message: {1}{0}Expected:\t{2}{0}But was:\t{3}",
                Environment.NewLine,
                string.IsNullOrEmpty(premessage) ? "Test assertion failed" : premessage,
                expected,
                actual);
        }

        #endregion

    }
}
