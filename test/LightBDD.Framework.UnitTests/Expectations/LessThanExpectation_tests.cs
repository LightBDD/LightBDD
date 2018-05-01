using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class LessThanExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<int>("less than '2'",
                    x => x.LessThan(2))
                .WithMatchingValues(0, 1)
                .WithNotMatchingValue(2, "expected: less than '2', but got: '2'")
                .WithNotMatchingValue(3, "expected: less than '2', but got: '3'");

            yield return new ExpectationScenario<string>(
                    "less than 'ab'",
                    x => x.LessThan("ab"))
                .WithMatchingValues("aa", "aaa")
                .WithNotMatchingValue("ab", "expected: less than 'ab', but got: 'ab'")
                .WithNotMatchingValue("az", "expected: less than 'ab', but got: 'az'")
                .WithNotMatchingValue(null, "expected: less than 'ab', but got: '<null>'");
        }
    }
}