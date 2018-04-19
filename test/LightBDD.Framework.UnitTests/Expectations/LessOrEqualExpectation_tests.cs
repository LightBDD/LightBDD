using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class LessOrEqualExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<int>("less or equal '2'",
                    x => x.LessOrEqual(2),
                    x => x.LessOrEqual(2))
                .WithMatchingValues(0,1,2)
                .WithNotMatchingValue(3, "expected: less or equal '2', but got: '3'");

            yield return new ExpectationScenario<string>(
                    "less or equal 'ab'",
                    x => x.LessOrEqual("ab"),
                    x => x.LessOrEqual("ab"))
                .WithMatchingValues("aa","aaa", "ab")
                .WithNotMatchingValue("ac", "expected: less or equal 'ab', but got: 'ac'")
                .WithNotMatchingValue(null, "expected: less or equal 'ab', but got: '<null>'");
        }
    }
}