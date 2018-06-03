using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class EveryItemExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<IEnumerable<string>>(
                    "every item contains 'a'",
                    x => x.EveryItem(item => item.Contain('a')))
                .WithMatchingValues(new[] { "apple", "banana" }, new string[] { })
                .WithNotMatchingValue(null, "expected: every item contains 'a', but got: '<null>'")
                .WithNotMatchingValue(new[] { "mango", "cherry" }, "expected: every item contains 'a', but got: 'mango, cherry'\n\t[1]: expected: contains 'a', but got: 'cherry'");

            yield return new ExpectationScenario<string[]>(
                    "every item contains 'a'",
                    x => x.EveryItem(item => item.Contain('a')))
                .WithMatchingValues(new[] { "apple", "banana" }, new string[] { })
                .WithNotMatchingValue(null, "expected: every item contains 'a', but got: '<null>'")
                .WithNotMatchingValue(new[] { "mango", "cherry" }, "expected: every item contains 'a', but got: 'mango, cherry'\n\t[1]: expected: contains 'a', but got: 'cherry'");
        }
    }
}