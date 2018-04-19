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
            yield return new ExpectationScenario<string>("matching regex 'fi.e[0-9]+.txt'",
                    x => x.MatchRegex("fi.e[0-9]+.txt"),
                    x => x.MatchRegex("fi.e[0-9]+.txt"))
                .WithMatchingValues("file1.txt", "file123.txt", "fine123.txt", "afile1.txt", "fine123.txt2")
                .WithNotMatchingValue(null, "expected: matching regex 'fi.e[0-9]+.txt', but got: '<null>'")
                .WithNotMatchingValue("File.txt", "expected: matching regex 'fi.e[0-9]+.txt', but got: 'File.txt'")
                .WithNotMatchingValue("afile.txt", "expected: matching regex 'fi.e[0-9]+.txt', but got: 'afile.txt'");

            yield return new ExpectationScenario<string>("matching regex any case 'fi.e[0-9]+.txt'",
                    x => x.MatchRegexIgnoreCase("fi.e[0-9]+.txt"),
                    x => x.MatchRegexIgnoreCase("fi.e[0-9]+.txt"))
                .WithMatchingValues("fILE1.tXt", "fIle123.txt", "FINE123.txt", "afile1.txt", "fine123.txt2")
                .WithNotMatchingValue(null, "expected: matching regex any case 'fi.e[0-9]+.txt', but got: '<null>'")
                .WithNotMatchingValue("afile.txt", "expected: matching regex any case 'fi.e[0-9]+.txt', but got: 'afile.txt'");
        }
    }
}