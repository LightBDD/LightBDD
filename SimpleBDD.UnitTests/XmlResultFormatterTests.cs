using System;
using NUnit.Framework;
using SimpleBDD.Results;
using SimpleBDD.Results.Formatters;

namespace SimpleBDD.UnitTests
{
	[TestFixture]
	public class XmlResultFormatterTests
	{
		private IResultFormatter _subject;

		[SetUp]
		public void SetUp()
		{
			_subject = new XmlResultFormatter();
		}

		[Test]
		public void Should_format_xml()
		{
			var result = new FeatureResult();
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, 2, "step1", ResultStatus.Passed), new StepResult(2, 2, "step2", ResultStatus.Ignored) }));
			result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, 2, "step3", ResultStatus.Passed), new StepResult(2, 2, "step4", ResultStatus.Failed) }));
			var text = _subject.Format(result);
			Console.WriteLine(text);

			const string expectedText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestResults xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Feature>
    <Scenario Name=""name"" Status=""Ignored"">
      <Step Name=""step1"" Status=""Passed"" StepNumber=""1"" TotalStepsCount=""2"" />
      <Step Name=""step2"" Status=""Ignored"" StepNumber=""2"" TotalStepsCount=""2"" />
    </Scenario>
    <Scenario Name=""name2"" Status=""Failed"">
      <Step Name=""step3"" Status=""Passed"" StepNumber=""1"" TotalStepsCount=""2"" />
      <Step Name=""step4"" Status=""Failed"" StepNumber=""2"" TotalStepsCount=""2"" />
    </Scenario>
  </Feature>
</TestResults>";
			Assert.That(text, Is.EqualTo(expectedText));
		}
	}
}