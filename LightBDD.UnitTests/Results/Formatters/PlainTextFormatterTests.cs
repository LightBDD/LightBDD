using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.Results.Implementation;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results.Formatters
{
	[TestFixture]
	public class PlainTextResultFormatterTests
	{
		private IResultFormatter _subject;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new PlainTextResultFormatter();
		}

		#endregion

		[Test]
		public void Should_format_feature_with_description()
		{
			var result = new FeatureResult("My feature","My feature\r\nlong description");
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, "step1", ResultStatus.Passed), new StepResult(2, "step2", ResultStatus.Ignored) }));
			result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, "step3", ResultStatus.Passed), new StepResult(2, "step4", ResultStatus.Failed) }));
			var text = _subject.Format(result);
			const string expectedText = @"Feature: My feature
	My feature
	long description

	Scenario: name - Ignored
		Step 1: step1 - Passed
		Step 2: step2 - Ignored

	Scenario: name2 - Failed
		Step 1: step3 - Passed
		Step 2: step4 - Failed
";
			Assert.That(text, Is.EqualTo(expectedText));
		}

		[Test]
		public void Should_format_feature_without_description()
		{
			var result = new FeatureResult("My feature", null);
			result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, "step1", ResultStatus.Passed), new StepResult(2, "step2", ResultStatus.Ignored) }));
			var text = _subject.Format(result);
			const string expectedText = @"Feature: My feature

	Scenario: name - Ignored
		Step 1: step1 - Passed
		Step 2: step2 - Ignored
";
			Assert.That(text, Is.EqualTo(expectedText));
		}
	}
}
