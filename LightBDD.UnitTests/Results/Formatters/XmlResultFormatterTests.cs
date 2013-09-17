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
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, "step1", ResultStatus.Passed), new StepResult(2, "step2", ResultStatus.Ignored) }, "Label 2"));
			result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, "step3", ResultStatus.Passed), new StepResult(2, "step4", ResultStatus.Failed) }, null));
			var text = _subject.Format(result);
			Console.WriteLine(text);

			const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Feature Name=""Feature name"" Label=""Label 1"">
    <Description>feature
long description</Description>
    <Scenario Status=""Ignored"" Name=""name"" Label=""Label 2"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" />
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"">
      <Step Status=""Passed"" Number=""1"" Name=""step3"" />
      <Step Status=""Failed"" Number=""2"" Name=""step4"" />
    </Scenario>
  </Feature>
</TestResults>";
			Assert.That(text, Is.EqualTo(expectedText));
		}

		[Test]
		public void Should_format_xml_without_description_nor_label()
		{
			var result = new FeatureResult("Feature name", null, null);
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, "step1", ResultStatus.Passed), new StepResult(2, "step2", ResultStatus.Ignored) }, null));
			result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, "step3", ResultStatus.Passed), new StepResult(2, "step4", ResultStatus.Failed) }, null));
			var text = _subject.Format(result);
			Console.WriteLine(text);

			const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults>
  <Feature Name=""Feature name"">
    <Scenario Status=""Ignored"" Name=""name"">
      <Step Status=""Passed"" Number=""1"" Name=""step1"" />
      <Step Status=""Ignored"" Number=""2"" Name=""step2"" />
    </Scenario>
    <Scenario Status=""Failed"" Name=""name2"">
      <Step Status=""Passed"" Number=""1"" Name=""step3"" />
      <Step Status=""Failed"" Number=""2"" Name=""step4"" />
    </Scenario>
  </Feature>
</TestResults>";
			Assert.That(text, Is.EqualTo(expectedText));
		}
	}
}