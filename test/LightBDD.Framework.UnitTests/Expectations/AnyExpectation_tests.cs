using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class AnyExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<IEnumerable<string>>(
                    "any contains 'a'",
                    x => x.Any(item => item.Contains('a')),
                    x => x.Any(item => item.Contains('a')))
                .WithMatchingValues(new[] { "apple", "banana" })
                .WithNotMatchingValue(null, "expected: any contains 'a', but got: '<null>'")
                .WithNotMatchingValue(Enumerable.Empty<string>(), "expected: any contains 'a', but got: ''")
                .WithNotMatchingValue(new[] { "cherry", "melon" }, "expected: any contains 'a', but got: 'cherry, melon'\n\t[0]: expected: contains 'a', but got: 'cherry'\n\t[1]: expected: contains 'a', but got: 'melon'");

            yield return new ExpectationScenario<string[]>(
                    "any contains 'a'",
                    x => x.Any(item => item.Contains('a')),
                    x => x.Any(item => item.Contains('a')))
                .WithMatchingValues(new[] { "apple", "banana" })
                .WithNotMatchingValue(null, "expected: any contains 'a', but got: '<null>'")
                .WithNotMatchingValue(new[] { "cherry", "melon" }, "expected: any contains 'a', but got: 'cherry, melon'\n\t[0]: expected: contains 'a', but got: 'cherry'\n\t[1]: expected: contains 'a', but got: 'melon'");
        }
    }
}