using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HtmlAgilityPack;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Formatters;
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
Passed 1. call step1 ""arg1"" (1m 01s)
Ignored 2. step2 (1s 100ms)
Passed 2.1. substep 1 (100ms)
Passed 2.2. substep 2 (1s)
Ignored 2.3. substep 3 (0ms)
Failed 2.3.1. sub-substep 1
table1:
# Key X Y
Key1 1 2
Key2
12
4
(missing) Key3
&lt;none&gt;3
&lt;none&gt;6
(surplus) Key4
3&lt;none&gt;
6&lt;none&gt;
table2:
# Key X Y
Key1 1 2
Key2
12
4
(missing) Key3
&lt;none&gt;3
&lt;none&gt;6
(surplus) Key4
3&lt;none&gt;
6&lt;none&gt;
NotRun 2.3.2. sub-substep 2
Step 2: Not implemented yet
// Step 1: multiline
comment
// Step 1: comment 2
// Step 2.3: sub-comment
// Step 2.3.1: sub-sub-multiline
comment
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

        private string GetExpectedLightBddVersion()
        {
            return typeof(IBddRunner).GetTypeInfo().Assembly.GetName().Version.ToString(4);
        }

        private string FormatAndExtractText(params IFeatureResult[] results)
        {
            var formatted = FormatResults(results);
            var doc = new HtmlDocument();
            doc.LoadHtml(formatted);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            var builder = FormatAllNodes(new HtmlToPlainTextFormatter(), body);
            Debug.WriteLine(builder.ToString());
            return builder.ToString();
        }

        private static HtmlToPlainTextFormatter FormatAllNodes(HtmlToPlainTextFormatter builder, HtmlNode node)
        {
            builder.EnsureSeparatorFor(node);

            if (node.Name == "#text")
                builder.Append(node.InnerText);

            foreach (var childNode in node.ChildNodes)
                FormatAllNodes(builder, childNode);

            return builder.EnsureSeparatorFor(node);
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

    internal class HtmlToPlainTextFormatter
    {
        private readonly IEnumerable<string> _blockElements = new[] { "div", "tr", "table", "section", "article", "h1", "h2", "h3", "br" };
        private readonly IEnumerable<string> _inlineElements = new[] { "td", "th" };//browsers are treating td/th in special way while for span they put no spaces when copied to clipboard
        private readonly List<string> _lines = new List<string>();
        private readonly StringBuilder _current = new StringBuilder();

        public HtmlToPlainTextFormatter EnsureNewLine()
        {
            if (_current.Length > 0)
            {
                _lines.Add(_current.ToString().TrimEnd());
                _current.Clear();
            }
            return this;
        }

        public HtmlToPlainTextFormatter EnsureSpace()
        {
            if (_current.Length > 0 && !char.IsWhiteSpace(_current[_current.Length - 1]))
                _current.Append(' ');
            return this;
        }

        public HtmlToPlainTextFormatter EnsureSeparatorFor(HtmlNode node)
        {
            var name = node.Name.ToLowerInvariant();
            if (_blockElements.Contains(name))
                return EnsureNewLine();
            if (_inlineElements.Contains(name))
                return EnsureSpace();
            return this;
        }

        public HtmlToPlainTextFormatter Append(string text)
        {
            _current.Append(text);
            return this;
        }

        public override string ToString()
        {
            EnsureNewLine();//to flush current
            return string.Join(Environment.NewLine, _lines);
        }
    }
}