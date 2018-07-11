using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class BeBetweenExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<int>("between '2' and '5'",
                    x => x.BeBetween(2, 5))
                .WithMatchingValues(2, 3, 4, 5)
                .WithNotMatchingValue(6, "expected: between '2' and '5', but got: '6'")
                .WithNotMatchingValue(1, "expected: between '2' and '5', but got: '1'");

            yield return new ExpectationScenario<string>(
                "between 'aha' and 'ada'",
                x => x.BeBetween("aha", "ada"))
                .WithMatchingValues("ada", "aea", "afa", "aga", "aha", "adaa")
                .WithNotMatchingValue("aia", "expected: between 'aha' and 'ada', but got: 'aia'")
                .WithNotMatchingValue("aca", "expected: between 'aha' and 'ada', but got: 'aca'")
                .WithNotMatchingValue("ahaa", "expected: between 'aha' and 'ada', but got: 'ahaa'")
                .WithNotMatchingValue(null, "expected: between 'aha' and 'ada', but got: '<null>'");
        }
    }
}