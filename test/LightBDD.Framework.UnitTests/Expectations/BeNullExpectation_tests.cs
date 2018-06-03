using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class BeNullExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<object>("null",
                    x => x.BeNull())
                .WithMatchingValue(null)
                .WithNotMatchingValue(5, "expected: null, but got: '5'")
                .WithNotMatchingValue("abc", "expected: null, but got: 'abc'")
                .WithNotMatchingValue("", "expected: null, but got: ''");
        }
    }
}