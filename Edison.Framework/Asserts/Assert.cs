/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework.Enums;
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
            if (!expected.Equals(actual))
            {
                throw new AssertException(ExpectedActualMessage(message, expected, actual));
            }
        }

        public virtual void AreNotEqual(IComparable expected, IComparable actual, string message = null)
        {
            if (expected.Equals(actual))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not " + expected, actual));
            }
        }

        #endregion

        #region Same

        public virtual void AreSameReference(object expected, object actual, string message = null)
        {
            if (!object.ReferenceEquals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, expected, actual));
            }
        }

        public virtual void AreNotSameReference(object expected, object actual, string message = null)
        {
            if (object.ReferenceEquals(expected, actual))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not " + expected, actual));
            }
        }

        #endregion

        #region Instance

        public virtual void IsInstanceOf<T>(object value, string message = null)
        {
            if (value == default(object))
            {
                throw new AssertException(ExpectedActualMessage(message, typeof(T).Name, "NULL"));
            }

            if (!(value is T))
            {
                throw new AssertException(ExpectedActualMessage(message, typeof(T).Name, value.GetType().Name));
            }
        }

        public virtual void IsNotInstanceOf<T>(object value, string message = null)
        {
            if (value != default(object) && (value is T))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not " + typeof(T).Name, value.GetType().Name));
            }
        }

        #endregion

        #region Boolean

        public virtual void IsTrue(bool value, string message = null)
        {
            if (!value)
            {
                throw new AssertException(ExpectedActualMessage(message, "TRUE", value));
            }
        }

        public virtual void IsFalse(bool value, string message = null)
        {
            if (value)
            {
                throw new AssertException(ExpectedActualMessage(message, "FALSE", value));
            }
        }

        #endregion

        #region Greater Than

        public virtual void IsGreaterThan(IComparable value, IComparable greaterThanThis, string message = null)
        {
            if (value.CompareTo(greaterThanThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">" + greaterThanThis, value));
            }
        }

        public virtual void IsGreaterThanOrEqual(IComparable value, IComparable greaterThanOrEqualToThis, string message = null)
        {
            if (value.CompareTo(greaterThanOrEqualToThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">=" + greaterThanOrEqualToThis, value));
            }
        }

        public virtual void IsNotGreaterThan(IComparable value, IComparable notGreaterThanThis, string message = null)
        {
            if (value.CompareTo(notGreaterThanThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<=" + notGreaterThanThis, value));
            }
        }

        public virtual void IsNotGreaterThanOrEqual(IComparable value, IComparable notGreaterThanOrEqualToThis, string message = null)
        {
            if (value.CompareTo(notGreaterThanOrEqualToThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<" + notGreaterThanOrEqualToThis, value));
            }
        }

        #endregion

        #region Less Than

        public virtual void IsLessThan(IComparable value, IComparable lessThanThis, string message = null)
        {
            if (value.CompareTo(lessThanThis) >= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<" + lessThanThis, value));
            }
        }

        public virtual void IsLessThanOrEqual(IComparable value, IComparable lessThanOrEqualToThis, string message = null)
        {
            if (value.CompareTo(lessThanOrEqualToThis) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "<=" + lessThanOrEqualToThis, value));
            }
        }

        public virtual void IsNotLessThan(IComparable value, IComparable notLessThanThis, string message = null)
        {
            if (value.CompareTo(notLessThanThis) < 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">=" + notLessThanThis, value));
            }
        }

        public virtual void IsNotLessThanOrEqual(IComparable value, IComparable notLessThanOrEqualToThis, string message = null)
        {
            if (value.CompareTo(notLessThanOrEqualToThis) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, ">" + notLessThanOrEqualToThis, value));
            }
        }

        #endregion

        #region Files and Directories

        public virtual void FileExists(string path, string message = null)
        {
            if (!File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File exists: " + path, "File does not exist"));
            }
        }

        public virtual void FileDoesNotExist(string path, string message = null)
        {
            if (File.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "File does not exist: " + path, "File exists"));
            }
        }

        public virtual void DirectoryExists(string path, string message = null)
        {
            if (!Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory exists: " + path, "Directory does not exist"));
            }
        }

        public virtual void DirectoryDoesNotExists(string path, string message = null)
        {
            if (Directory.Exists(path))
            {
                throw new AssertException(ExpectedActualMessage(message, "Directory does not exists: " + path, "Directory exists"));
            }
        }

        #endregion

        #region Null, Default and Types

        public virtual void IsNull(object value, string message = null)
        {
            if (value != null)
            {
                throw new AssertException(ExpectedActualMessage(message, "NULL", value));
            }
        }

        public virtual void IsNotNull(object value, string message = null)
        {
            if (value == null)
            {
                throw new AssertException(ExpectedActualMessage(message, "Not NULL", "NULL"));
            }
        }

        public virtual void IsDefault<T>(T value, string message = null)
        {
            if (!object.Equals(value, default(T)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Default " + typeof(T).Name, "Not default " + value.GetType().Name));
            }
        }

        public virtual void IsNotDefault<T>(T value, string message = null)
        {
            if (object.Equals(value, default(T)))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not default " + typeof(T).Name, "Default " + typeof(T).Name));
            }
        }

        public virtual void IsInstanceOfType(object value, Type type, string message = null)
        {
            if (!type.IsInstanceOfType(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Instance of type " + type.Name, "Not instance of type " + value.GetType().Name));
            }
        }

        public virtual void IsNotInstanceOfType(object value, Type type, string message = null)
        {
            if (type.IsInstanceOfType(value))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not instance of type " + type.Name, "Instance of type " + value.GetType().Name));
            }
        }

        #endregion

        #region Zero

        public virtual void IsZero(IComparable value, string message = null)
        {
            if (!value.Equals(0))
            {
                throw new AssertException(ExpectedActualMessage(message, "0", value));
            }
        }

        public virtual void IsNotZero(IComparable value, string message = null)
        {
            if (value.Equals(0))
            {
                throw new AssertException(ExpectedActualMessage(message, "Not 0", value));
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
            if (value.CompareTo(lowerBound) < 0 || value.CompareTo(upperBound) > 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Between " + lowerBound + " and " + upperBound, value));
            }
        }

        public virtual void IsNotBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null)
        {
            if (value.CompareTo(lowerBound) >= 0 && value.CompareTo(upperBound) <= 0)
            {
                throw new AssertException(ExpectedActualMessage(message, "Not between " + lowerBound + " and " + upperBound, value));
            }
        }

        #endregion

        #region Protected Helpers

        protected string ExpectedActualMessage(string premessage, object expected, object actual)
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
