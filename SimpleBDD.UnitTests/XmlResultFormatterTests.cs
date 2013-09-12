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
		public void Should_format_plain_text()
		{
			var result = new StoryResult();
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, 2, "step1", ResultStatus.Passed), new StepResult(2, 2, "step2", ResultStatus.Ignored) }));
			result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, 2, "step3", ResultStatus.Passed), new StepResult(2, 2, "step4", ResultStatus.Failed) }));
			var text = _subject.Format(result);
			Console.WriteLine(text);
			const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<StoryResult xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Scenarios Name=""name"" Status=""Ignored"">
    <Steps Name=""step1"" Status=""Passed"" StepNumber=""1"" TotalStepsCount=""2"" />
    <Steps Name=""step2"" Status=""Ignored"" StepNumber=""2"" TotalStepsCount=""2"" />
  </Scenarios>
  <Scenarios Name=""name2"" Status=""Failed"">
    <Steps Name=""step3"" Status=""Passed"" StepNumber=""1"" TotalStepsCount=""2"" />
    <Steps Name=""step4"" Status=""Failed"" StepNumber=""2"" TotalStepsCount=""2"" />
  </Scenarios>
</StoryResult>";
			Assert.That(text, Is.EqualTo(expectedText));
		}
	}
}