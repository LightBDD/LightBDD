using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.Results.Implementation;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
    [TestFixture]
    public class HtmlResultFormatterTests
    {
        private IResultFormatter _subject;
        private DateTimeOffset _startDate = new DateTimeOffset(2014, 09, 23, 19, 21, 57, 55, TimeSpan.Zero);
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
            var result = new FeatureResult("My feature", string.Format("My feature{0}long description", Environment.NewLine), "Label 1");
            result.AddScenario(new ScenarioResult("name", new[]
                {
                    new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(new TimeSpan(0, 1, 1)).SetExecutionStart(_startDate.AddSeconds(2)), 
                    new StepResult(2, "step2", ResultStatus.Ignored, "Not implemented yet").SetExecutionTime(new TimeSpan(0, 0, 0, 1, 100)).SetExecutionStart(_startDate.AddSeconds(3))
                }, "Label 2").SetExecutionTime(new TimeSpan(0, 0, 1, 2, 100)).SetExecutionStart(_startDate.AddSeconds(1)));
            result.AddScenario(new ScenarioResult("name2", new[]
                {
                    new StepResult(1, "step3", ResultStatus.Passed).SetExecutionTime(new TimeSpan(0, 0, 0, 2, 107)).SetExecutionStart(_startDate.AddSeconds(5)), 
                    new StepResult(2, "step4", ResultStatus.Failed, string.Format("  Expected: True{0}  But was: False", Environment.NewLine)).SetExecutionTime(new TimeSpan(0, 0, 0, 0, 50)).SetExecutionStart(_startDate.AddSeconds(6)),
                    new StepResult(3, "step5", ResultStatus.NotRun)
                }, null).SetExecutionTime(new TimeSpan(0, 0, 0, 2, 157)).SetExecutionStart(_startDate.AddSeconds(4)));

            var text = FormatAndExtractText(result);
            const string expectedText = @"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution time: 1m 04s
Number of features: 1
Number of scenarios: 2
Passed scenarios: 0
Ignored scenarios: 1
Failed scenarios: 1
Feature summary
Feature Scenarios Passed Ignored Failed Duration
My feature [Label 1] 2 0 1 1 1m 04s
Feature details
Filter: Passed Failed Ignored Not Run
My feature [Label 1]
My feature
long description
Ignored name [Label 2] (1m 02s)
Passed 1. step1 (1m 01s)
Ignored 2. step2 (1s 100ms)
Not implemented yet
Failed name2 (2s 157ms)
Passed 1. step3 (2s 107ms)
Failed 2. step4 (50ms)
NotRun 3. step5
Expected: True
  But was: False";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_feature_without_description_nor_label_nor_details()
        {
            var result = new FeatureResult("My feature", null, null);
            result.AddScenario(new ScenarioResult("name", new[]
                {
                    new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(2)), 
                    new StepResult(2, "step2", ResultStatus.Ignored).SetExecutionTime(TimeSpan.FromMilliseconds(5)).SetExecutionStart(_startDate.AddSeconds(3))
                }, null).SetExecutionTime(TimeSpan.FromMilliseconds(25)).SetExecutionStart(_startDate.AddSeconds(1)));

            var text = FormatAndExtractText(result);

            const string expectedText = @"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution time: 25ms
Number of features: 1
Number of scenarios: 1
Passed scenarios: 0
Ignored scenarios: 1
Failed scenarios: 0
Feature summary
Feature Scenarios Passed Ignored Failed Duration
My feature 1 0 1 0 25ms
Feature details
Filter: Passed Failed Ignored Not Run
My feature
Ignored name (25ms)
Passed 1. step1 (20ms)
Ignored 2. step2 (5ms)";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Multiple_features_should_be_separated_by_new_line()
        {
            var feature1 = new FeatureResult("My feature", null, null);
            feature1.AddScenario(new ScenarioResult("scenario1", new[]
                {
                    new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(2))
                }, null).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(1)));

            var feature2 = new FeatureResult("My feature2", null, null);
            feature2.AddScenario(new ScenarioResult("scenario1", new[]
                {
                    new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(5))
                }, null).SetExecutionTime(TimeSpan.FromMilliseconds(20)).SetExecutionStart(_startDate.AddSeconds(4)));

            var text = FormatAndExtractText(feature1, feature2);
            const string expectedText = @"Execution summary
Test execution start time: 2014-09-23 19:21:58 UTC
Test execution time: 40ms
Number of features: 2
Number of scenarios: 2
Passed scenarios: 2
Ignored scenarios: 0
Failed scenarios: 0
Feature summary
Feature Scenarios Passed Ignored Failed Duration
My feature 1 1 0 0 20ms
My feature2 1 1 0 0 20ms
Feature details
Filter: Passed Failed Ignored Not Run
My feature
Passed scenario1 (20ms)
Passed 1. step1 (20ms)
My feature2
Passed scenario1 (20ms)
Passed 1. step1 (20ms)";
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
        private readonly IEnumerable<string> _blockElements = new[] { "div", "tr", "table", "section", "article", "h1", "h2", "h3" };
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