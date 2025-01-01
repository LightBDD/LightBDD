using System.Collections.Generic;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.UnitTests.Expectations.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Expectations
{
    [TestFixture]
    public class BeLikeExpectation_tests : ExpectationTests
    {
        protected override IEnumerable<IExpectationScenario> GetScenarios()
        {
            yield return new ExpectationScenario<string>("like 'fi?e*.txt'",
                    x => x.BeLike("fi?e*.txt"))
                .WithMatchingValues("file.txt", "file123.txt", "fine123.txt", "fine.txt")
                .WithNotMatchingValue(null, "expected: like 'fi?e*.txt', but got: '<null>'")
                .WithNotMatchingValue("File.txt", "expected: like 'fi?e*.txt', but got: 'File.txt'")
                .WithNotMatchingValue("afile.txt", "expected: like 'fi?e*.txt', but got: 'afile.txt'");

            yield return new ExpectationScenario<string>("like 'no###'",
                    x => x.BeLike("no###"))
                .WithMatchingValues("no000", "no123")
                .WithNotMatchingValue(null, "expected: like 'no###', but got: '<null>'")
                .WithNotMatchingValue("no1", "expected: like 'no###', but got: 'no1'")
                .WithNotMatchingValue("no1234", "expected: like 'no###', but got: 'no1234'")
                .WithNotMatchingValue("noabc", "expected: like 'no###', but got: 'noabc'");

            yield return new ExpectationScenario<string>("like 'no###' ignore case",
                    x => x.BeLikeIgnoreCase("no###"))
                .WithMatchingValues("no000", "nO123", "No123")
                .WithNotMatchingValue(null, "expected: like 'no###' ignore case, but got: '<null>'")
                .WithNotMatchingValue("no1", "expected: like 'no###' ignore case, but got: 'no1'")
                .WithNotMatchingValue("no1234", "expected: like 'no###' ignore case, but got: 'no1234'")
                .WithNotMatchingValue("noabc", "expected: like 'no###' ignore case, but got: 'noabc'");

            yield return new ExpectationScenario<string>("like '<p>*</p>'",
                    x => x.BeLike("<p>*</p>"))
                .WithMatchingValues("<p>some text</p>", "<p>some\nmultiline\r\ntext</p>")
                .WithNotMatchingValue("<span>text</span>", "expected: like '<p>*</p>', but got: '<span>text</span>'")
                .WithNotMatchingValue("<P>text</P>", "expected: like '<p>*</p>', but got: '<P>text</P>'");

            yield return new ExpectationScenario<string>("like '<p>*</p>' ignore case",
                    x => x.BeLikeIgnoreCase("<p>*</p>"))
                .WithMatchingValues("<p>some text</p>", "<p>some\nmultiline\r\ntext</p>", "<P>text</P>")
                .WithNotMatchingValue("<span>text</span>", "expected: like '<p>*</p>' ignore case, but got: '<span>text</span>'");
        }
    }
}