/*
Edison is designed to be simpler and more performant unit/integration testing framework.

Copyright (c) 2015, Matthew Kelly (Badgerati)
Company: Cadaeic Studios
License: MIT (see LICENSE for details)
 */

using Edison.Framework;
using Edison.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edison.Engine.Utilities.Helpers
{
    public static class ReflectionHelper
    {

        public static bool HasValidAttributes<T>(IEnumerable<Attribute> attributes, IList<string> includedCategories, IList<string> excludedCategories)
        {
            return attributes != default(IEnumerable<Attribute>)
                && attributes.OfType<T>().Any()
                && !attributes.OfType<IgnoreAttribute>().Any()
                && HasValidCategories(attributes, includedCategories, excludedCategories);
        }

        public static bool HasValidCategories(IEnumerable<Attribute> attributes, IList<string> includedCategories, IList<string> excludedCategories)
        {
            if (includedCategories == default(List<string>) && excludedCategories == default(List<string>))
            {
                return true;
            }

            var categories = attributes.OfType<CategoryAttribute>();
            var isTestFixture = attributes.OfType<TestFixtureAttribute>().Any();

            if (isTestFixture && !categories.Any())
            {
                return true;
            }

            if (includedCategories != default(List<string>) && categories.Any(c => includedCategories.Any(i => i.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return true;
            }

            if (excludedCategories != default(List<string>) && categories.Any(c => excludedCategories.Any(e => e.Equals(c.Name, StringComparison.InvariantCultureIgnoreCase))))
            {
                return false;
            }

            if (isTestFixture)
            {
                return true;
            }

            if (includedCategories != default(List<string>) && includedCategories.Any())
            {
                return false;
            }

            return true;
        }

        public static bool HasValidConcurrency(IEnumerable<Attribute> attributes, ConcurrencyType concurrencyType, ConcurrencyType defaultConcurreny = ConcurrencyType.Parallel)
        {
            var concurrency = attributes.OfType<ConcurrencyAttribute>().FirstOrDefault();

            return concurrency == default(ConcurrencyAttribute)
                ? concurrencyType == defaultConcurreny
                : concurrency.ConcurrencyType == concurrencyType;
        }

    }
}
