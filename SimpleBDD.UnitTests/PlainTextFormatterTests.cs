using NUnit.Framework;
using SimpleBDD.Results;
using SimpleBDD.Results.Formatters;
using SimpleBDD.Results.Implementation;

namespace SimpleBDD.UnitTests
{
	[TestFixture]
	public class PlainTextResultFormatterTests
	{
		private IResultFormatter _subject;

		[SetUp]
		public void SetUp()
		{
			_subject = new PlainTextResultFormatter();
		}

		[Test]
		public void Should_format_plain_text()
		{
			var result = new FeatureResult();
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, 2, "step1", ResultStatus.Passed), new StepResult(2, 2, "step2", ResultStatus.Ignored) }));
			result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, 2, "step3", ResultStatus.Passed), new StepResult(2, 2, "step4", ResultStatus.Failed) }));
			var text = _subject.Format(result);
			const string expectedText = @"Scenario: name - Ignored
	Step 1/2: step1 - Passed
	Step 2/2: step2 - Ignored

Scenario: name2 - Failed
	Step 1/2: step3 - Passed
	Step 2/2: step4 - Failed
";
			Assert.That(text, Is.EqualTo(expectedText));
		}
	}
}
