/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edison.Framework
{
    public interface IAssert
    {

        void Inconclusive(string message = null);
        void Fail(string message = null);
        void Pass(string message = null);

        void AreEqual(IComparable expected, IComparable actual, string message = null);
        void AreNotEqual(IComparable expected, IComparable actual, string message = null);

        void AreSameReference(object expected, object actual, string message = null);
        void AreNotSameReference(object expected, object actual, string message = null);

        void IsInstanceOf<T>(object value, string message = null);
        void IsNotInstanceOf<T>(object value, string message = null);

        void IsTrue(bool value, string message = null);
        void IsFalse(bool value, string message = null);

        void IsGreaterThan(IComparable value, IComparable greaterThanThis, string message = null);
        void IsGreaterThanOrEqual(IComparable value, IComparable greaterThanOrEqualToThis, string message = null);
        void IsNotGreaterThan(IComparable value, IComparable notGreaterThanThis, string message = null);
        void IsNotGreaterThanOrEqual(IComparable value, IComparable notGreaterThanOrEqualToThis, string message = null);

        void IsLessThan(IComparable value, IComparable lessThanThis, string message = null);
        void IsLessThanOrEqual(IComparable value, IComparable lessThanOrEqualToThis, string message = null);
        void IsNotLessThan(IComparable value, IComparable notLessThanThis, string message = null);
        void IsNotLessThanOrEqual(IComparable value, IComparable notLessThanOrEqualToThis, string message = null);

        void FileExists(string path, string message = null);
        void FileDoesNotExist(string path, string message = null);

        void DirectoryExists(string path, string message = null);
        void DirectoryDoesNotExists(string path, string message = null);

        void IsNull(object value, string message = null);
        void IsNotNull(object value, string message = null);

        void IsDefault<T>(T value, string message = null);
        void IsNotDefault<T>(T value, string message = null);

        void IsInstanceOfType(object value, Type type, string message = null);
        void IsNotInstanceOfType(object value, Type type, string message = null);

        void IsZero(IComparable value, string message = null);
        void IsNotZero(IComparable value, string message = null);

        void AreDatesEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null);
        void AreDatesNotEqual(DateTime expected, DateTime actual, int minuteOffset = 0, string message = null);

        void AreTimesEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null);
        void AreTimesNotEqual(TimeSpan expected, TimeSpan actual, TimeSpan offset = default(TimeSpan), string message = null);

        void IsBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null);
        void IsNotBetween(IComparable value, IComparable lowerBound, IComparable upperBound, string message = null);

    }
}
