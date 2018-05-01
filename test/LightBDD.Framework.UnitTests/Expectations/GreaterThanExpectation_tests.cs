using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class GreaterThanExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<int>("greater than '2'",
                    x => x.GreaterThan(2))
                .WithMatchingValues(3, 4, 5)
                .WithNotMatchingValue(2, "expected: greater than '2', but got: '2'")
                .WithNotMatchingValue(1, "expected: greater than '2', but got: '1'");

            yield return new ExpectationScenario<string>(
                    "greater than 'ab'",
                    x => x.GreaterThan("ab"))
                .WithMatchingValues("aba", "ab ", "az","b")
                .WithNotMatchingValue("ab", "expected: greater than 'ab', but got: 'ab'")
                .WithNotMatchingValue("aa", "expected: greater than 'ab', but got: 'aa'")
                .WithNotMatchingValue(null, "expected: greater than 'ab', but got: '<null>'");
        }
    }
}