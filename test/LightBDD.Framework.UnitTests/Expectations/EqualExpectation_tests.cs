using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class EqualExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<string>(
                    "equal 'abc'",
                    x => x.Equal("abc"),
                    x => x.Equal("abc"))
                .WithMatchingValues("abc")
                .WithNotMatchingValue(null, "expected: equal 'abc', but got: '<null>'")
                .WithNotMatchingValue("Abc", "expected: equal 'abc', but got: 'Abc'")
                .WithNotMatchingValue("Something", "expected: equal 'abc', but got: 'Something'");
        }
    }
}