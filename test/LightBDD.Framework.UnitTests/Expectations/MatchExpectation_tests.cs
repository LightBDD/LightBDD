using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class MatchExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<string>("matches 'fi?e*.txt'",
                    x => x.Match("fi?e*.txt"))
                .WithMatchingValues("file.txt", "file123.txt", "fine123.txt", "fine.txt")
                .WithNotMatchingValue(null, "expected: matches 'fi?e*.txt', but got: '<null>'")
                .WithNotMatchingValue("File.txt", "expected: matches 'fi?e*.txt', but got: 'File.txt'")
                .WithNotMatchingValue("afile.txt", "expected: matches 'fi?e*.txt', but got: 'afile.txt'");

            yield return new ExpectationScenario<string>("matches 'no###'",
                    x => x.Match("no###"))
                .WithMatchingValues("no000", "no123")
                .WithNotMatchingValue(null, "expected: matches 'no###', but got: '<null>'")
                .WithNotMatchingValue("no1", "expected: matches 'no###', but got: 'no1'")
                .WithNotMatchingValue("no1234", "expected: matches 'no###', but got: 'no1234'")
                .WithNotMatchingValue("noabc", "expected: matches 'no###', but got: 'noabc'");

            yield return new ExpectationScenario<string>("matches 'no###' ignore case",
                    x => x.MatchIgnoreCase("no###"))
                .WithMatchingValues("no000", "nO123", "No123")
                .WithNotMatchingValue(null, "expected: matches 'no###' ignore case, but got: '<null>'")
                .WithNotMatchingValue("no1", "expected: matches 'no###' ignore case, but got: 'no1'")
                .WithNotMatchingValue("no1234", "expected: matches 'no###' ignore case, but got: 'no1234'")
                .WithNotMatchingValue("noabc", "expected: matches 'no###' ignore case, but got: 'noabc'");
        }
    }
}