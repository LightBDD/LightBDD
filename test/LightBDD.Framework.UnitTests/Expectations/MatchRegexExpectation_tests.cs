using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class MatchRegexExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<string>(
                    "matches regex 'fi.e[0-9]+.txt'",
                    x => x.MatchRegex("fi.e[0-9]+.txt"))
                .WithMatchingValues("file1.txt", "file123.txt", "fine123.txt")
                .WithNotMatchingValue(null, "expected: matches regex 'fi.e[0-9]+.txt', but got: '<null>'")
                .WithNotMatchingValue("File.txt", "expected: matches regex 'fi.e[0-9]+.txt', but got: 'File.txt'")
                .WithNotMatchingValue("file.txt", "expected: matches regex 'fi.e[0-9]+.txt', but got: 'file.txt'")
                .WithNotMatchingValue("afile1.txt", "expected: matches regex 'fi.e[0-9]+.txt', but got: 'afile1.txt'")
                .WithNotMatchingValue("fine123.txt2", "expected: matches regex 'fi.e[0-9]+.txt', but got: 'fine123.txt2'");


            yield return new ExpectationScenario<string>(
                    "matches regex 'fi.e[0-9]+.txt' ignore case",
                    x => x.MatchRegexIgnoreCase("fi.e[0-9]+.txt"))
                .WithMatchingValues("fILE1.tXt", "fIle123.txt", "FINE123.txt")
                .WithNotMatchingValue(null, "expected: matches regex 'fi.e[0-9]+.txt' ignore case, but got: '<null>'")
                .WithNotMatchingValue("file.txt", "expected: matches regex 'fi.e[0-9]+.txt' ignore case, but got: 'file.txt'")
                .WithNotMatchingValue("afile1.txt", "expected: matches regex 'fi.e[0-9]+.txt' ignore case, but got: 'afile1.txt'")
                .WithNotMatchingValue("fine123.txt2", "expected: matches regex 'fi.e[0-9]+.txt' ignore case, but got: 'fine123.txt2'");

            yield return new ExpectationScenario<string>(
                    "matches regex '.*some-text.*'",
                    x => x.MatchRegex(".*some-text.*"))
                .WithMatchingValues("some-text", "awesome-texture");
        }
    }
}