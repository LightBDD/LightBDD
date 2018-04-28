using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class InExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<string>(
                        "in 'banana, orange, pear'",
                        x => x.In("banana", "orange", "pear"),
                        x => x.In("banana", "orange", "pear"))
                    .WithMatchingValues("banana", "orange", "pear")
                    .WithNotMatchingValue(null, "expected: in 'banana, orange, pear', but got: '<null>'")
                    .WithNotMatchingValue("apple", "expected: in 'banana, orange, pear', but got: 'apple'");
        }
    }
}