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
                    x => x.MatchWild("fi?e*.txt"))
                .WithMatchingValues("file.txt", "file123.txt", "fine123.txt", "fine.txt")
                .WithNotMatchingValue(null, "expected: matching 'fi?e*.txt', but got: '<null>'")
                .WithNotMatchingValue("File.txt", "expected: matching 'fi?e*.txt', but got: 'File.txt'")
                .WithNotMatchingValue("afile.txt", "expected: matching 'fi?e*.txt', but got: 'afile.txt'");

            yield return new ExpectationScenario<string>("matching 'no###'",
                    x => x.MatchWild("no###"))
                .WithMatchingValues("no000", "no123")
                .WithNotMatchingValue(null, "expected: matching 'no###', but got: '<null>'")
                .WithNotMatchingValue("no1", "expected: matching 'no###', but got: 'no1'")
                .WithNotMatchingValue("no1234", "expected: matching 'no###', but got: 'no1234'")
                .WithNotMatchingValue("noabc", "expected: matching 'no###', but got: 'noabc'");

            yield return new ExpectationScenario<string>("matching any case 'no###'",
                    x => x.MatchWildIgnoreCase("no###"))
                .WithMatchingValues("no000", "nO123", "No123")
                .WithNotMatchingValue(null, "expected: matching any case 'no###', but got: '<null>'")
                .WithNotMatchingValue("no1", "expected: matching any case 'no###', but got: 'no1'")
                .WithNotMatchingValue("no1234", "expected: matching any case 'no###', but got: 'no1234'")
                .WithNotMatchingValue("noabc", "expected: matching any case 'no###', but got: 'noabc'");
        }
    }
}