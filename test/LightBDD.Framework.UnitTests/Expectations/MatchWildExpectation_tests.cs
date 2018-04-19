using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class MatchWildExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<string>("matching 'fi?e*.txt'",
                    x => x.MatchWild("fi?e*.txt"),
                    x => x.MatchWild("fi?e*.txt"))
                .WithMatchingValues("file.txt", "file123.txt", "fine123.txt", "fine.txt")
                .WithNotMatchingValue(null, "expected: matching 'fi?e*.txt', but got: '<null>'")
                .WithNotMatchingValue("File.txt", "expected: matching 'fi?e*.txt', but got: 'File.txt'")
                .WithNotMatchingValue("afile.txt", "expected: matching 'fi?e*.txt', but got: 'afile.txt'");
        }
    }
}