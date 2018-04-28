using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class ContainsExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<IEnumerable<string>>(
                    "contains 'banana'",
                    x => x.Contains("banana"),
                    x => x.Contains("banana"))
                .WithMatchingValues(new[] { "apple", "banana" })
                .WithNotMatchingValue(null, "expected: contains 'banana', but got: '<null>'")
                .WithNotMatchingValue(Enumerable.Empty<string>(), "expected: contains 'banana', but got: ''")
                .WithNotMatchingValue(new[] { "apple", "orange" }, "expected: contains 'banana', but got: 'apple, orange'");

            yield return new ExpectationScenario<string>(
                    "contains 'c'",
                    x => x.Contains('c'),
                    x => x.Contains('c'))
                .WithMatchingValues("oracle", "colosseum")
                .WithNotMatchingValue("faro", "expected: contains 'c', but got: 'faro'")
                .WithNotMatchingValue(null, "expected: contains 'c', but got: '<null>'");
        }
    }
}