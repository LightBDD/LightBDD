﻿using System;
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
            var expectedText = $@"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution end time: 2014-09-23 19:23:00 UTC
Test execution time: 1m 02s
Test execution time (aggregated): 1m 04s
Number of features: 1
Number of scenarios: 2
Passed scenarios: 0
Bypassed scenarios: 0
Failed scenarios: 1 (see details)
Ignored scenarios: 1 (see details)
Number of steps: 10
Passed steps: 3
Bypassed steps: 1
Failed steps: 2
Ignored steps: 2
Not Run steps: 2
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature [Label 1] 2 0 0 1 1 10 3 1 2 2 2 1m 02s 621000000 1m 04s 642570000 32s 128ms 321285000
Feature details[&#8734;link]
Toggle: Features Scenarios
Filter: Passed Bypassed Failed Ignored Not Run
Categories: -all- categoryA categoryB categoryC -without category-
[&#8734;filtered link]
My feature [Label 1][&#8734;link]
My feature
long description
Ignored name [Label 2] (1m 02s)[&#8734;link]
categoryA
Passed 1. call step1 &quot;arg1&quot; (1m 01s)
Ignored 2. step2 (1s 100ms)
Passed 2.1. substep 1 (100ms)
Passed 2.2. substep 2 (1s)
Ignored 2.3. substep 3 (0ms)
Failed 2.3.1. sub-substep 1
table1:
# Key X Y
= Key1 1 2
! Key2
12
4
(missing) Key3
&lt;none&gt;3
&lt;none&gt;6
(surplus) Key4
3&lt;none&gt;
6&lt;none&gt;
table2:
Key X Y
Key1 1 2
Key2 2 4
Key3 3 6
NotRun 2.3.2. sub-substep 2
Step 2: Not implemented yet
// Step 1: multiline
comment
// Step 1: comment 2
// Step 2.3: sub-comment
// Step 2.3.1: sub-sub-multiline
comment
&#128279;Step 2.3.1: attachment1 (png)
Failed name2 ""arg1"" (2s 157ms)[&#8734;link]
categoryB, categoryC
Bypassed 1. step3 (2s 107ms)
Failed 2. step4 (50ms)
NotRun 3. step5
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
            var expectedText = $@"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution end time: 2014-09-23 19:21:58 UTC
Test execution time: 25ms
Test execution time (aggregated): 25ms
Number of features: 1
Number of scenarios: 1
Passed scenarios: 0
Bypassed scenarios: 0
Failed scenarios: 0
Ignored scenarios: 1 (see details)
Number of steps: 2
Passed steps: 1
Bypassed steps: 0
Failed steps: 0
Ignored steps: 1
Not Run steps: 0
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature 1 0 0 0 1 2 1 0 0 1 0 25ms 250000 25ms 250000 25ms 250000
Feature details[&#8734;link]
Toggle: Features Scenarios
Filter: Passed Bypassed Failed Ignored Not Run
[&#8734;filtered link]
My feature[&#8734;link]
Ignored name (25ms)[&#8734;link]
Passed 1. step1 (20ms)
Ignored 2. step2 (5ms)
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var results = ReportFormatterTestData.GetMultipleFeatureResults();
            var text = FormatAndExtractText(results);
            TestContext.WriteLine(text);
            var expectedText = $@"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution end time: 2014-09-23 19:22:01 UTC
Test execution time: 3s 020ms
Test execution time (aggregated): 40ms
Number of features: 2
Number of scenarios: 2
Passed scenarios: 2
Bypassed scenarios: 0
Failed scenarios: 0
Ignored scenarios: 0
Number of steps: 2
Passed steps: 2
Bypassed steps: 0
Failed steps: 0
Ignored steps: 0
Not Run steps: 0
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature 1 1 0 0 0 1 1 0 0 0 0 20ms 200000 20ms 200000 20ms 200000
My feature2 1 1 0 0 0 1 1 0 0 0 0 20ms 200000 20ms 200000 20ms 200000
Feature details[&#8734;link]
Toggle: Features Scenarios
Filter: Passed Bypassed Failed Ignored Not Run
Categories: -all- categoryA categoryB -without category-
[&#8734;filtered link]
My feature[&#8734;link]
Passed scenario1 (20ms)[&#8734;link]
categoryA
Passed 1. step1 (20ms)
My feature2[&#8734;link]
Passed scenario1 (20ms)[&#8734;link]
categoryB
Passed 1. step1 (20ms)
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_escape_html_characters_in_step_name()
        {
            var date = new DateTimeOffset(2019, 10, 21, 05, 06, 07, TimeSpan.Zero);
            var feature = TestResults.CreateFeatureResult("My feature", null, null,
                TestResults.CreateScenarioResult("scenario1", null, date, TimeSpan.FromSeconds(5), new string[0],
                    TestResults.CreateStepResult(ExecutionStatus.Passed)
                        .WithStepNameDetails(1, "ste<p>", "ste<p>", "ty<p>e")
                        .WithGroupPrefix("<gr>")
                        .WithExecutionTime(date, TimeSpan.FromMilliseconds(20)),
                    TestResults.CreateStepResult(ExecutionStatus.Passed)
                        .WithStepNameDetails(2, "ste<p>2", "ste<p>2")
                        .WithExecutionTime(date.AddMilliseconds(50), TimeSpan.FromMilliseconds(20))));

            var text = FormatAndExtractText(feature);
            TestContext.WriteLine(text);
            var expectedText = $@"Execution summary
Test execution start time: 2019-10-21 05:06:07 UTC
Test execution end time: 2019-10-21 05:06:12 UTC
Test execution time: 5s
Test execution time (aggregated): 5s
Number of features: 1
Number of scenarios: 1
Passed scenarios: 1
Bypassed scenarios: 0
Failed scenarios: 0
Ignored scenarios: 0
Number of steps: 2
Passed steps: 2
Bypassed steps: 0
Failed steps: 0
Ignored steps: 0
Not Run steps: 0
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My feature 1 1 0 0 0 2 2 0 0 0 0 5s 50000000 5s 50000000 5s 50000000
Feature details[&#8734;link]
Toggle: Features Scenarios
Filter: Passed Bypassed Failed Ignored Not Run
[&#8734;filtered link]
My feature[&#8734;link]
Passed scenario1 (5s)[&#8734;link]
Passed &lt;gr&gt;1. ty&lt;p&gt;e ste&lt;p&gt; (20ms)
Passed 2. ste&lt;p&gt;2 (20ms)
Generated with LightBDD v{GetExpectedLightBddVersion()}
initialize();";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        [Test]
        public void Should_format_scenarios_in_order()
        {
            var results = ReportFormatterTestData.GetFeatureWithUnsortedScenarios();
            var text = FormatAndExtractText(results);
            TestContext.WriteLine(text);
            var expectedText = $@"Execution summary
Test execution start time: 2014-09-23 19:21:57 UTC
Test execution end time: 2014-09-23 19:22:02 UTC
Test execution time: 5s
Test execution time (aggregated): 9s
Number of features: 1
Number of scenarios: 3
Passed scenarios: 3
Bypassed scenarios: 0
Failed scenarios: 0
Ignored scenarios: 0
Number of steps: 3
Passed steps: 3
Bypassed steps: 0
Failed steps: 0
Ignored steps: 0
Not Run steps: 0
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Aggregated Average
My Feature 3 3 0 0 0 3 3 0 0 0 0 5s 50000000 9s 90000000 3s 30000000
Feature details[&#8734;link]
Toggle: Features Scenarios
Filter: Passed Bypassed Failed Ignored Not Run
[&#8734;filtered link]
My Feature[&#8734;link]
Passed scenario A [lab B] (2s)[&#8734;link]
Passed 1. step
Passed scenario B [lab C] (5s)[&#8734;link]
Passed 1. step
Passed scenario C [lab A] (2s)[&#8734;link]
Passed 1. step
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

            var tree = new VerifiableTree(expected, new());
            tree.SetActual(actual);

            var results = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTableText("//table[@class=\"param tree\"]", results);
            TestContext.WriteLine(text);

            var expectedText = @"! | $ | &lt;object&gt; | Name | Johnny / John | Surname | Johnson |
! | ↳ | Address | &lt;object&gt; | City | London | Country | UK | PostCode | AB1 7BC / AB1 7BA | Street | High Street |
! | ↳ | Records | &lt;array:4&gt; / &lt;array:3&gt; | [0] | AB-1 | [1] | AB-2 | [2] | AB-3 | [3] | AB-4 / &lt;none&gt; |";
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

            var results = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTableText("//table[@class=\"param tree\"]", results);
            TestContext.WriteLine(text);

            var expectedText = @"$ | &lt;object&gt; | Name | John | Surname | Johnson |
↳ | Address | &lt;object&gt; | City | London | Country | UK | PostCode | AB1 7BA | Street | High Street |
↳ | Records | &lt;array:3&gt; | [0] | AB-1 | [1] | AB-2 | [2] | AB-3 |";
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

            var tree = new VerifiableTree(input, new());
            tree.SetActual(input);

            var results = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTableText("//table[@class=\"param tree\"]", results);
            TestContext.WriteLine(text);

            var expectedText = @"= | $ | &lt;array:3&gt; |
= | ↳ | [0] | &lt;array:5&gt; | [0] | 0 | [1] | 1 | [2] | 2 | [3] | 3 | [4] | 4 |
= | ↳ | [1] | &lt;array:6&gt; | [0] | 0 | [1] | 1 | [2] | 2 | [3] | 3 | [4] | 4 | [5] | 5 |
= | ↳ | [2] | &lt;array:4&gt; | [0] | 0 | [1] | 1 | [2] | 2 | [3] | 3 |";
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

            var tree = new VerifiableTree(input, new());
            tree.SetActual(input);

            var results = ReportFormatterTestData.GetFeatureWithVerifiableTree(tree.Details);
            var text = FormatAndExtractTableText("//table[@class=\"param tree\"]", results);
            TestContext.WriteLine(text);

            var expectedText = @"= | $ | &lt;array:3&gt; |
= | ↳ | [0] | &lt;object&gt; | Name | Bob | Surname | Kennedy |
= | ↳ | [1] | &lt;object&gt; | Name | Ron | Surname | Kowalski |
= | ↳ | [2] | &lt;object&gt; | Name | Ted | Surname | Smith |";
            Assert.That(text.NormalizeNewLine(), Is.EqualTo(expectedText.NormalizeNewLine()));
        }

        private string GetExpectedLightBddVersion()
        {
            return typeof(IBddRunner).GetTypeInfo().Assembly.GetName().Version.ToString(4);
        }

        private string FormatAndExtractText(params IFeatureResult[] results)
        {
            var formatted = FormatResults(results);
            var doc = new HtmlDocument();
            doc.LoadHtml(formatted);
            var builder = new HtmlToPlainTextFormatter();
            builder.FormatNode(doc.DocumentNode.SelectSingleNode("//body"));
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }

        private string FormatAndExtractTableText(string xpath, params IFeatureResult[] results)
        {
            var formatted = FormatResults(results);
            var doc = new HtmlDocument();
            doc.LoadHtml(formatted);
            var builder = new HtmlTableToPlainTextFormatter();
            builder.FormatNode(doc.DocumentNode.SelectSingleNode(xpath));
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }

        private string FormatResults(params IFeatureResult[] results)
        {
            using (var memory = new MemoryStream())
            {
                _subject.Format(memory, results);
                return Encoding.UTF8.GetString(memory.ToArray());
            }
        }
    }
}