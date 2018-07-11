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
                    "equals 'abc'",
                    x => x.Equal("abc"))
                .WithMatchingValues("abc")
                .WithNotMatchingValue(null, "expected: equals 'abc', but got: '<null>'")
                .WithNotMatchingValue("Abc", "expected: equals 'abc', but got: 'Abc'")
                .WithNotMatchingValue("Something", "expected: equals 'abc', but got: 'Something'");
        }
    }
}