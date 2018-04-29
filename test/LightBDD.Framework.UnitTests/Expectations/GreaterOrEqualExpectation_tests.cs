using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class GreaterOrEqualExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<int>("greater or equal '2'",
                    x => x.GreaterOrEqual(2))
                .WithMatchingValues(2, 3, 4, 5)
                .WithNotMatchingValue(1, "expected: greater or equal '2', but got: '1'");

            yield return new ExpectationScenario<string>(
                    "greater or equal 'ab'",
                    x => x.GreaterOrEqual("ab"))
                .WithMatchingValues("ab", "aba", "ab ", "az", "b")
                .WithNotMatchingValue("aa", "expected: greater or equal 'ab', but got: 'aa'")
                .WithNotMatchingValue(null, "expected: greater or equal 'ab', but got: '<null>'");
        }
    }
}