﻿using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class EqualCollectionExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<IEnumerable<string>>(
                    "equals collection 'banana, apple'",
                    x => x.EqualCollection("banana", "apple"))
                .WithMatchingValues(new[] { "banana", "apple" })
                .WithNotMatchingValue(null, "expected: equals collection 'banana, apple', but got: '<null>'")
                .WithNotMatchingValue(new[] { "banana", "Apple" }, "expected: equals collection 'banana, apple', but got: 'banana, Apple'\n\t[1]: expected: 'apple', but got: 'Apple'")
                .WithNotMatchingValue(new[] { "apple", "banana" }, "expected: equals collection 'banana, apple', but got: 'apple, banana'\n\t[0]: expected: 'banana', but got: 'apple'\n\t[1]: expected: 'apple', but got: 'banana'")
                .WithNotMatchingValue(Enumerable.Empty<string>(), "expected: equals collection 'banana, apple', but got: '<empty>'\n\texpected collection of 2 item(s), but got one of 0 item(s)\n\t[0]: missing: 'banana'\n\t[1]: missing: 'apple'")
                .WithNotMatchingValue(new[] { "banana", "pear", "orange" }, "expected: equals collection 'banana, apple', but got: 'banana, pear, orange'\n\texpected collection of 2 item(s), but got one of 3 item(s)\n\t[1]: expected: 'apple', but got: 'pear'\n\t[2]: surplus: 'orange'");

            yield return new ExpectationScenario<IEnumerable<int>>(
                    "equals collection '5, 4, -3'",
                    x => x.EqualCollection(5, 4, -3))
                .WithMatchingValues(new[] {5, 4, -3},new List<int>{5,4,-3});
        }
    }
}