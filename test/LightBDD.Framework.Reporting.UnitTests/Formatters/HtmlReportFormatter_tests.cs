using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;
using LightBDD.Core.Results;
using LightBDD.Framework.Parameters;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.Framework.Reporting.UnitTests.Formatters.Helpers;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

namespace LightBDD.Framework.Reporting.UnitTests.Formatters
{
    [TestFixture]
    public class HtmlReportFormatter_tests
    {
        private IReportFormatter _subject;
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new HtmlReportFormatter();
        }

        #endregion

        [Test]
        public void Should_format_feature_with_description()
        {
            var result = ReportFormatterTestData.GetFeatureResultWithDescription();

            var text = FormatAndExtractText(result);
            TestContext.WriteLine(text);
            var expectedText = $@"Test execution summary
Execution
Test suite: Random.Tests
Overall status: Failed
Start date: 2014-09-23 (UTC)
Start time: 19:21:58
End time: 19:23:00
Duration: 1m 02s
Content
Features: 1
Scenarios: 2
Passed scenarios: 0
Failed scenarios: 1 (see details)
Ignored scenarios: 1 (see details)
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature Label 1 2 0 0 1 1 10 3 1 2 2 2 1m 02s 621000000 1m 04s 642570000 32s 128ms 321285000
Totals 2 0 0 1 1 10 3 1 2 2 2 1m 02s 621000000 1m 04s 642570000 32s 128ms 321285000
Feature details link
Toggle: Features Scenarios Sub Steps
Filter: Passed Bypassed Failed Ignored Not Run
Categories: -all- categoryA categoryB categoryC -without category-
 filtered link
My feature Label 1 link
My feature
long description
! name Label 2 (1m 02s) link
 categoryA
✓ 1. call step1 &quot;arg1&quot; (1m 01s)
! 2. step2 (1s 100ms)
✓ 2.1. substep 1 (100ms)
✓ 2.2. substep 2 (1s)
! 2.3. substep 3 (0ms)
✕ 2.3.1. sub-substep 1
table1:
# Key X Y
= Key1 1 2
! Key2 1/2 4
- Key3 &lt;none&gt;/3 &lt;none&gt;/6
+ Key4 3/&lt;none&gt; 6/&lt;none&gt;
table2:
Key X Y
Key1 1 2
Key2 2 4
Key3 3 6
? 2.3.2. sub-substep 2
Details:
Step 2: Not implemented yet
Comments:
// Step 1: multiline
comment
// Step 1: comment 2
// Step 2.3: sub-comment
// Step 2.3.1: sub-sub-multiline
comment
Attachments:
&#128279;Step 2.3.1: attachment1 (png)
✕ name2 ""arg1"" (2s 157ms) link
 categoryB categoryC
~ 1. step3 (2s 107ms)
✕ 2. step4 (50ms)
? 3. step5
Details:
Step 1: bypass reason
Step 2: Expected: True
	  But was: False
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_feature_without_description_nor_label_nor_details()
        {
            var result = ReportFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();

            var text = FormatAndExtractText(result);
            TestContext.WriteLine(text);
            var expectedText = $@"Test execution summary
Execution
Test suite: Random.Tests
Overall status: Passed
Start date: 2014-09-23 (UTC)
Start time: 19:21:58
End time: 19:21:58
Duration: 25ms
Content
Features: 1
Scenarios: 1
Passed scenarios: 0
Ignored scenarios: 1 (see details)
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature 1 0 0 0 1 2 1 0 0 1 0 25ms 250000 25ms 250000 25ms 250000
Totals 1 0 0 0 1 2 1 0 0 1 0 25ms 250000 25ms 250000 25ms 250000
Feature details link
Toggle: Features Scenarios Sub Steps
Filter: Passed Bypassed Failed Ignored Not Run
 filtered link
My feature link
! name (25ms) link
✓ 1. step1 (20ms)
! 2. step2 (5ms)
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var result = ReportFormatterTestData.GetMultipleFeatureResults();
            var text = FormatAndExtractText(result);
            TestContext.WriteLine(text);
            var expectedText = $@"Test execution summary
Execution
Test suite: Random.Tests
Overall status: Passed
Start date: 2014-09-23 (UTC)
Start time: 19:21:58
End time: 19:22:01
Duration: 3s 020ms
Content
Features: 2
Scenarios: 2
Passed scenarios: 2
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature 1 1 0 0 0 1 1 0 0 0 0 20ms 200000 20ms 200000 20ms 200000
My feature2 1 1 0 0 0 1 1 0 0 0 0 20ms 200000 20ms 200000 20ms 200000
Totals 2 2 0 0 0 2 2 0 0 0 0 3s 020ms 30200000 40ms 400000 20ms 200000
Feature details link
Toggle: Features Scenarios Sub Steps
Filter: Passed Bypassed Failed Ignored Not Run
Categories: -all- categoryA categoryB -without category-
 filtered link
My feature link
✓ scenario1 (20ms) link
 categoryA
✓ 1. step1 (20ms)
My feature2 link
✓ scenario1 (20ms) link
 categoryB
✓ 1. step1 (20ms)
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_escape_html_characters_in_step_name()
        {
            var date = new DateTimeOffset(2019, 10, 21, 05, 06, 07, TimeSpan.Zero);
            var result = TestResults.CreateTestRunResults(TestResults.CreateFeatureResult("My feature", null, null,
                TestResults.CreateScenarioResult("scenario1", null, date, TimeSpan.FromSeconds(5), Array.Empty<string>(),
                    TestResults.CreateStepResult(ExecutionStatus.Passed)
                        .WithStepNameDetails(1, "ste<p>", "ste<p>", "ty<p>e")
                        .WithGroupPrefix("<gr>")
                        .WithExecutionTime(date, TimeSpan.FromMilliseconds(20)),
                    TestResults.CreateStepResult(ExecutionStatus.Passed)
                        .WithStepNameDetails(2, "ste<p>2", "ste<p>2")
                        .WithExecutionTime(date.AddMilliseconds(50), TimeSpan.FromMilliseconds(20)))));

            var text = FormatAndExtractText(result);
            TestContext.WriteLine(text);
            var expectedText = $@"Test execution summary
Execution
Test suite: Random.Tests
Overall status: Passed
Start date: 2019-10-21 (UTC)
Start time: 05:06:07
End time: 05:06:12
Duration: 5s
Content
Features: 1
Scenarios: 1
Passed scenarios: 1
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature 1 1 0 0 0 2 2 0 0 0 0 5s 50000000 5s 50000000 5s 50000000
Totals 1 1 0 0 0 2 2 0 0 0 0 5s 50000000 5s 50000000 5s 50000000
Feature details link
Toggle: Features Scenarios Sub Steps
Filter: Passed Bypassed Failed Ignored Not Run
 filtered link
My feature link
✓ scenario1 (5s) link
✓ &lt;gr&gt;1. ty&lt;p&gt;e ste&lt;p&gt; (20ms)
✓ 2. ste&lt;p&gt;2 (20ms)
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_scenarios_in_order()
        {
            var result = ReportFormatterTestData.GetFeatureWithUnsortedScenarios();
            var text = FormatAndExtractText(result);
            TestContext.WriteLine(text);
            var expectedText = $@"Test execution summary
Execution
Test suite: Random.Tests
Overall status: Passed
Start date: 2014-09-23 (UTC)
Start time: 19:21:57
End time: 19:22:02
Duration: 5s
Content
Features: 1
Scenarios: 3
Passed scenarios: 3
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My Feature 3 3 0 0 0 3 3 0 0 0 0 5s 50000000 9s 90000000 3s 30000000
Totals 3 3 0 0 0 3 3 0 0 0 0 5s 50000000 9s 90000000 3s 30000000
Feature details link
Toggle: Features Scenarios Sub Steps
Filter: Passed Bypassed Failed Ignored Not Run
 filtered link
My Feature link
✓ scenario A lab B (2s) link
✓ 1. step
✓ scenario B lab C (5s) link
✓ 1. step
✓ scenario C lab A (2s) link
✓ 1. step
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_verifiable_trees()
        {
            var expected = new
            {
                Name = "John",
                Surname = "Johnson",
                Address = new { Street = "High Street", PostCode = "AB1 7BA", City = "London", Country = "UK" },
                Records = new[] { "AB-1", "AB-2", "AB-3" }
            };
            var actual = new
            {
                Name = "Johnny",
                Surname = "Johnson",
                Address = new { Street = "High Street", PostCode = "AB1 7BC", City = "London", Country = "UK" },
                Records = new[] { "AB-1", "AB-2", "AB-3", "AB-4" }
            };

            var tree = Tree.ExpectEquivalent(expected);
            tree.SetActual(actual);

            var results = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTreeText("//div[@class=\"param tree\"]", results);
            TestContext.WriteLine(text);

            var expectedText = @"$=&lt;object&gt;
$.Name=JohnnyJohn
$.Surname=Johnson
$.Address=&lt;object&gt;
$.Address.City=London
$.Address.Country=UK
$.Address.PostCode=AB1 7BCAB1 7BA
$.Address.Street=High Street
$.Records=&lt;array:4&gt;&lt;array:3&gt;
$.Records.[0]=AB-1
$.Records.[1]=AB-2
$.Records.[2]=AB-3
$.Records.[3]=AB-4&lt;none&gt;";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_input_trees()
        {
            var input = new
            {
                Name = "John",
                Surname = "Johnson",
                Address = new { Street = "High Street", PostCode = "AB1 7BA", City = "London", Country = "UK" },
                Records = new[] { "AB-1", "AB-2", "AB-3" }
            };

            var tree = Tree.For(input);

            var result = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTreeText("//div[@class=\"param tree\"]", result);
            TestContext.WriteLine(text);

            var expectedText = @"$=&lt;object&gt;
$.Name=John
$.Surname=Johnson
$.Address=&lt;object&gt;
$.Address.City=London
$.Address.Country=UK
$.Address.PostCode=AB1 7BA
$.Address.Street=High Street
$.Records=&lt;array:3&gt;
$.Records.[0]=AB-1
$.Records.[1]=AB-2
$.Records.[2]=AB-3";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_verifiable_tree_jagged_arrays()
        {
            var input = new[]
            {
                Enumerable.Range(0,5),
                Enumerable.Range(0,6),
                Enumerable.Range(0,4)
            };

            var tree = Tree.ExpectEquivalent(input);
            tree.SetActual(input);

            var result = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTreeText("//div[@class=\"param tree\"]", result);
            TestContext.WriteLine(text);

            var expectedText = @"$=&lt;array:3&gt;
$.[0]=&lt;array:5&gt;
$.[0].[0]=0
$.[0].[1]=1
$.[0].[2]=2
$.[0].[3]=3
$.[0].[4]=4
$.[1]=&lt;array:6&gt;
$.[1].[0]=0
$.[1].[1]=1
$.[1].[2]=2
$.[1].[3]=3
$.[1].[4]=4
$.[1].[5]=5
$.[2]=&lt;array:4&gt;
$.[2].[0]=0
$.[2].[1]=1
$.[2].[2]=2
$.[2].[3]=3";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_verifiable_tree_array_with_sub_objects()
        {
            var input = new[]
            {
                new{Name="Bob",Surname="Kennedy"},
                new{Name="Ron",Surname="Kowalski"},
                new{Name="Ted",Surname="Smith"},
            };

            var tree = Tree.ExpectEquivalent(input);
            tree.SetActual(input);

            var result = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTreeText("//div[@class=\"param tree\"]", result);
            TestContext.WriteLine(text);

            var expectedText = @"$=&lt;array:3&gt;
$.[0]=&lt;object&gt;
$.[0].Name=Bob
$.[0].Surname=Kennedy
$.[1]=&lt;object&gt;
$.[1].Name=Ron
$.[1].Surname=Kowalski
$.[2]=&lt;object&gt;
$.[2].Name=Ted
$.[2].Surname=Smith";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        private string GetExpectedLightBddVersion()
        {
            return typeof(IBddRunner).GetTypeInfo().Assembly.GetName().Version.ToString(4);
        }

        private string FormatAndExtractText(ITestRunResult result)
        {
            var formatted = FormatResult(result);
            var doc = new HtmlDocument();
            doc.LoadHtml(formatted);
            var builder = new HtmlToPlainTextFormatter();
            builder.FormatNode(doc.DocumentNode.SelectSingleNode("//body"));
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }

        private string FormatAndExtractTreeText(string xpath, ITestRunResult result)
        {
            var formatted = FormatResult(result);
            var doc = new HtmlDocument();
            doc.LoadHtml(formatted);
            var builder = new HtmlTreeToPlainTextFormatter();
            builder.FormatTree(doc.DocumentNode.SelectSingleNode(xpath));
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }

        private string FormatResult(ITestRunResult result)
        {
            using (var memory = new MemoryStream())
            {
                _subject.Format(memory, result);
                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }
    }
}