using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            yield return new ExpectationScenario<string>(
                    "matches 'fi.e[0-9]+.txt'",
                    x => x.Match("fi.e[0-9]+.txt"))
                .WithMatchingValues("file1.txt", "file123.txt", "fine123.txt")
                .WithNotMatchingValue(null, "expected: matches 'fi.e[0-9]+.txt', but got: '<null>'")
                .WithNotMatchingValue("File.txt", "expected: matches 'fi.e[0-9]+.txt', but got: 'File.txt'")
                .WithNotMatchingValue("file.txt", "expected: matches 'fi.e[0-9]+.txt', but got: 'file.txt'")
                .WithNotMatchingValue("afile1.txt", "expected: matches 'fi.e[0-9]+.txt', but got: 'afile1.txt'")
                .WithNotMatchingValue("fine123.txt2", "expected: matches 'fi.e[0-9]+.txt', but got: 'fine123.txt2'");

            yield return new ExpectationScenario<string>(
                    "matches 'fi.e[0-9]+.txt' ignore case",
                    x => x.MatchIgnoreCase("fi.e[0-9]+.txt"))
                .WithMatchingValues("fILE1.tXt", "fIle123.txt", "FINE123.txt")
                .WithNotMatchingValue(null, "expected: matches 'fi.e[0-9]+.txt' ignore case, but got: '<null>'")
                .WithNotMatchingValue("file.txt", "expected: matches 'fi.e[0-9]+.txt' ignore case, but got: 'file.txt'")
                .WithNotMatchingValue("afile1.txt", "expected: matches 'fi.e[0-9]+.txt' ignore case, but got: 'afile1.txt'")
                .WithNotMatchingValue("fine123.txt2", "expected: matches 'fi.e[0-9]+.txt' ignore case, but got: 'fine123.txt2'");

            yield return new ExpectationScenario<string>(
                    "matches '.*some-text.*'",
                    x => x.Match(".*some-text.*"))
                .WithMatchingValues("some-text", "awesome-texture");

            yield return new ExpectationScenario<string>(
                    "matches '<p>.*</p>'",
                    x => x.Match("<p>.*</p>"))
                .WithMatchingValues("<p>single line</p>")
                .WithNotMatchingValue("<p>multi\nline</p>", "expected: matches '<p>.*</p>', but got: '<p>multi\nline</p>'");

            yield return new ExpectationScenario<string>(
                    "matches '<p>.*</p>'",
                    x => x.Match("<p>.*</p>", RegexOptions.Singleline))
                .WithMatchingValues("<p>multi\nline</p>", "<p>single line</p>");
        }
    }
}