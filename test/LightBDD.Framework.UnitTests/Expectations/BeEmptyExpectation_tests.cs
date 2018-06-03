using System.Collections.Generic;
using System.Linq;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class BeEmptyExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<string>(
                    "empty",
                    x => x.BeEmpty())
                .WithMatchingValues(string.Empty, "")
                .WithNotMatchingValue(null, "expected: empty, but got: '<null>'")
                .WithNotMatchingValue("abc", "expected: empty, but got: 'abc'");

            yield return new ExpectationScenario<IEnumerable<int>>(
                    "empty",
                    x => x.BeEmpty())
                .WithMatchingValues(new int[0], Enumerable.Empty<int>(), new List<int>())
                .WithNotMatchingValue(null, "expected: empty, but got: '<null>'")
                .WithNotMatchingValue(new[] { 5 }, "expected: empty, but got: '5'");
        }
    }
}