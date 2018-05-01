using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class AllExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<IEnumerable<string>>(
                    "all contains 'a'",
                    x => x.All(item => item.Contains('a')))
                .WithMatchingValues(new[] { "apple", "banana" }, new string[] { })
                .WithNotMatchingValue(null, "expected: all contains 'a', but got: '<null>'")
                .WithNotMatchingValue(new[] { "mango", "cherry" }, "expected: all contains 'a', but got: 'mango, cherry'\n\t[1]: expected: contains 'a', but got: 'cherry'");

            yield return new ExpectationScenario<string[]>(
                    "all contains 'a'",
                    x => x.All(item => item.Contains('a')))
                .WithMatchingValues(new[] { "apple", "banana" }, new string[] { })
                .WithNotMatchingValue(null, "expected: all contains 'a', but got: '<null>'")
                .WithNotMatchingValue(new[] { "mango", "cherry" }, "expected: all contains 'a', but got: 'mango, cherry'\n\t[1]: expected: contains 'a', but got: 'cherry'");
        }
    }
}