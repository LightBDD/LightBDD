using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations;

[TestFixture]
public class BeOfType_tests : ExpectationTests
{
    protected override IEnumerable<IExpectationScenario> GetScenarios()
    {
        yield return new ExpectationScenario<object>(
                "be of type 'String'",
                x => x.BeOfType<string>())
            .WithMatchingValues("abc", "")
            .WithNotMatchingValue(null, "expected: be of type 'String', but got type '<null>'")
            .WithNotMatchingValue('c', "expected: be of type 'String', but got type 'Char'");
    }
}