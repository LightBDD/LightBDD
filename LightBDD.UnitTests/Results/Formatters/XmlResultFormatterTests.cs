using System;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.Results.Implementation;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
    [TestFixture]
    public class XmlResultFormatterTests
    {
        private IResultFormatter _subject;
        private DateTimeOffset _startDate = new DateTimeOffset(2014, 09, 23, 19, 21, 57, 55, TimeSpan.Zero);
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new XmlResultFormatter();
        }

        #endregion

        [Test]
        public void Should_format_xml()
        {
            var result = new FeatureResult("Feature name", "feature\nlong description", "Label 1");
            result.AddScenario(new ScenarioResult("name", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromSeconds(65)).SetExecutionStart(_startDate.AddSeconds(2)), 
                new StepResult(2, "step2", ResultStatus.Ignored, "Not implemented yet").SetExecutionTime(TimeSpan.FromSeconds(65)).SetExecutionStart(_startDate.AddSeconds(3))
            }, "Label 2").SetExecutionTime(TimeSpan.FromSeconds(130)).SetExecutionStart(_startDate));
            result.AddScenario(new ScenarioResult("name2", new[]
            {
                new StepResult(1, "step3", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(150)).SetExecutionStart(_startDate.AddSeconds(2)), 
                new StepResult(2, "step4", ResultStatus.Failed,"  Expected: True\n  But was: False").SetExecutionTime(TimeSpan.FromMilliseconds(250)).SetExecutionStart(_startDate.AddSeconds(3))
            }, null).SetExecutionTime(TimeSpan.FromMilliseconds(400)).SetExecutionStart(_startDate.AddSeconds(1)));
            var text = _subject.Format(result);
            Console.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Feature Name=""Feature name"" Label=""Label 1"">
    <Description>feature
long description</Description>
    <Scenario Status=""Ignored"" Name=""name"" Label=""Label 2"" ExecutionStart=""2014-09-23T19:21:57.055Z"" ExecutionTime=""PT2M10S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT1M5S"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT1M5S"" />
      <StatusDetails>Not implemented yet</StatusDetails>
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT0.4S"">
      <Step Status=""Passed"" Number=""1"" Name=""step3"" ExecutionStart=""2014-09-23T19:21:59.055Z"" ExecutionTime=""PT0.15S"" />
      <Step Status=""Failed"" Number=""2"" Name=""step4"" ExecutionStart=""2014-09-23T19:22:00.055Z"" ExecutionTime=""PT0.25S"" />
      <StatusDetails>  Expected: True
  But was: False</StatusDetails>
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_xml_without_description_nor_label_nor_details()
        {
            var result = new FeatureResult("Feature name", null, null);
            result.AddScenario(new ScenarioResult("name", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromSeconds(65)).SetExecutionStart(_startDate), 
                new StepResult(2, "step2", ResultStatus.Ignored).SetExecutionTime(TimeSpan.FromSeconds(65)).SetExecutionStart(_startDate)
            }, null).SetExecutionTime(TimeSpan.FromSeconds(130)).SetExecutionStart(_startDate.AddSeconds(1)));
            result.AddScenario(new ScenarioResult("name2", new[]
            {
                new StepResult(1, "step3", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromSeconds(65)).SetExecutionStart(_startDate), 
                new StepResult(2, "step4", ResultStatus.Failed).SetExecutionTime(TimeSpan.FromTicks(1)).SetExecutionStart(_startDate)
            }, null).SetExecutionTime(TimeSpan.FromSeconds(130)).SetExecutionStart(_startDate.AddSeconds(1)));
            var text = _subject.Format(result);
            Console.WriteLine(text);

            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Feature Name=""Feature name"">
    <Scenario Status=""Ignored"" Name=""name"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT2M10S"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" ExecutionStart=""2014-09-23T19:21:57.055Z"" ExecutionTime=""PT1M5S"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" ExecutionStart=""2014-09-23T19:21:57.055Z"" ExecutionTime=""PT1M5S"" />
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"" ExecutionStart=""2014-09-23T19:21:58.055Z"" ExecutionTime=""PT2M10S"">
      <Step Status=""Passed"" Number=""1"" Name=""step3"" ExecutionStart=""2014-09-23T19:21:57.055Z"" ExecutionTime=""PT1M5S"" />
      <Step Status=""Failed"" Number=""2"" Name=""step4"" ExecutionStart=""2014-09-23T19:21:57.055Z"" ExecutionTime=""PT0.0000001S"" />
    </Scenario>
  </Feature>
</TestResults>";
            Assert.That(text, Is.EqualTo(expectedText));
        }
    }
}