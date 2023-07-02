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


            yield return new ExpectationScenario<object>(
                    "equals '3'",
                    x => x.Equal((object)(byte)3))
                .WithMatchingValues((byte)3, (sbyte)3, 3, (uint)3, 3L, 3UL, 3f, 3d, 3m, 3)
                .WithNotMatchingValue(null, "expected: equals '3', but got: '<null>'")
                .WithNotMatchingValue(3.1, "expected: equals '3', but got: '3.1'")
                .WithNotMatchingValue(-3, "expected: equals '3', but got: '-3'");

            yield return new ExpectationScenario<object>(
                    "equals '-2147483648'",
                    x => x.Equal((object)int.MinValue))
                .WithMatchingValues(int.MinValue, (long)int.MinValue)
                .WithNotMatchingValue(unchecked((uint)int.MinValue), "expected: equals '-2147483648', but got: '2147483648'");
        }
    }
}