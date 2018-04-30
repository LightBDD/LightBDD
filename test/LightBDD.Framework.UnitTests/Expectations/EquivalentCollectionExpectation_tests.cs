using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class EquivalentCollectionExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<IEnumerable<string>>(
                    "equivalent collection 'banana, apple'",
                    x => x.EquivalentCollection("banana", "apple"))
                .WithMatchingValues(new[] { "banana", "apple" }, new[] { "apple", "banana" })
                .WithNotMatchingValue(null, "expected: equivalent collection 'banana, apple', but got: '<null>'")
                .WithNotMatchingValue(new[] { "Apple", "banana" }, "expected: equivalent collection 'banana, apple', but got: 'Apple, banana'\n\tmissing: 'apple'\n\tsurplus: 'Apple'")
                .WithNotMatchingValue(Enumerable.Empty<string>(), "expected: equivalent collection 'banana, apple', but got: ''\n\texpected collection of 2 item(s), but got one of 0 item(s)\n\tmissing: 'banana'\n\tmissing: 'apple'")
                .WithNotMatchingValue(new[] { "pear", "orange", "banana" }, "expected: equivalent collection 'banana, apple', but got: 'pear, orange, banana'\n\texpected collection of 2 item(s), but got one of 3 item(s)\n\tmissing: 'apple'\n\tsurplus: 'pear'\n\tsurplus: 'orange'")
                .WithNotMatchingValue(new[] { "banana", "banana" }, "expected: equivalent collection 'banana, apple', but got: 'banana, banana'\n\tmissing: 'apple'\n\tsurplus: 'banana'");
        }
    }
}