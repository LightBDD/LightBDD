using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
    [TestFixture]
    public class HtmlResultFormatterTests
    {
        private IResultFormatter _subject;
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new HtmlResultFormatter();
        }

        #endregion

        [Test]
        public void Should_format_feature_with_description()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithDescription();

            var text = FormatAndExtractText(result);
            const string expectedText = @"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution time: 1m 04s
Number of features: 1
Number of scenarios: 2
Passed scenarios: 0
Bypassed scenarios: 0
Failed scenarios: 1 (see details)
Ignored scenarios: 1 (see details)
Number of steps: 5
Passed steps: 1
Bypassed steps: 1
Failed steps: 1
Ignored steps: 1
Not Run steps: 1
Feature summary
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Average
My feature [Label 1] 2 0 0 1 1 5 1 1 1 1 1 1m 04s 642570000 32s 128ms 321285000
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
Passed 1. step1 (1m 01s)
Ignored 2. step2 (1s 100ms)
Step 2: Not implemented yet
Failed name2 (2s 157ms)[&#8734;link]
categoryB, categoryC
Bypassed 1. step3 (2s 107ms)
Failed 2. step4 (50ms)
NotRun 3. step5
Step 1: bypass reason
Step 2: Expected: True
	  But was: False
Generated with LightBDD
initialize();";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_feature_without_description_nor_label_nor_details()
        {
            var result = ResultFormatterTestData.GetFeatureResultWithoutDescriptionNorLabelNorDetails();

            var text = FormatAndExtractText(result);

            const string expectedText = @"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution time: 25ms
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
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Average
My feature 1 0 0 0 1 2 1 0 0 1 0 25ms 250000 25ms 250000
Feature details[&#8734;link]
Toggle: Features Scenarios
Filter: Passed Bypassed Failed Ignored Not Run
[&#8734;filtered link]
My feature[&#8734;link]
Ignored name (25ms)[&#8734;link]
Passed 1. step1 (20ms)
Ignored 2. step2 (5ms)
Generated with LightBDD
initialize();";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_multiple_features()
        {
            var results = ResultFormatterTestData.GetMultipleFeatureResults();
            var text = FormatAndExtractText(results);
            const string expectedText = @"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution time: 40ms
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
Feature Scenarios Passed Bypassed Failed Ignored Steps Passed Bypassed Failed Ignored Not Run Duration Average
My feature 1 1 0 0 0 1 1 0 0 0 0 20ms 200000 20ms 200000
My feature2 1 1 0 0 0 1 1 0 0 0 0 20ms 200000 20ms 200000
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
Generated with LightBDD
initialize();";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        private string FormatAndExtractText(params IFeatureResult[] results)
        {
            var formatted = _subject.Format(results);
            var doc = new HtmlDocument();
            doc.LoadHtml(formatted);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            var builder = FormatAllNodes(new HtmlToPlainTextFormatter(), body);
            Console.WriteLine(builder.ToString());
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
    }

    class HtmlToPlainTextFormatter
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