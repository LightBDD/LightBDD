using System;
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
            var result = new FeatureResult("My feature", string.Format("My feature{0}long description", Environment.NewLine), "Label 1");
            result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, "step1", ResultStatus.Passed), new StepResult(2, "step2", ResultStatus.Ignored, "Not implemented yet") }, "Label 2"));
            result.AddScenario(new ScenarioResult("name2", new[] { new StepResult(1, "step3", ResultStatus.Passed), new StepResult(2, "step4", ResultStatus.Failed, string.Format("  Expected: True{0}  But was: False", Environment.NewLine)) }, null));
            var text = _subject.Format(result);
            const string expectedText = @"Feature: [Label 1] My feature
	My feature
	long description

	Scenario: [Label 2] name - Ignored
		Step 1: step1 - Passed
		Step 2: step2 - Ignored

		Details: Not implemented yet

	Scenario: name2 - Failed
		Step 1: step3 - Passed
		Step 2: step4 - Failed

		Details: Expected: True
			  But was: False
";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_feature_without_description_nor_label_nor_details()
        {
            var result = new FeatureResult("My feature", null, null);
            result.AddScenario(new ScenarioResult("name", new[] { new StepResult(1, "step1", ResultStatus.Passed), new StepResult(2, "step2", ResultStatus.Ignored) }, null));
            var text = _subject.Format(result);
            const string expectedText = @"Feature: My feature

	Scenario: name - Ignored
		Step 1: step1 - Passed
		Step 2: step2 - Ignored
";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Multiple_features_should_be_separated_by_new_line()
        {
            var feature1 = new FeatureResult("My feature", null, null);
            feature1.AddScenario(new ScenarioResult("scenario1", new[] { new StepResult(1, "step1", ResultStatus.Passed) }, null));
            var feature2 = new FeatureResult("My feature2", null, null);
            feature2.AddScenario(new ScenarioResult("scenario1", new[] { new StepResult(1, "step1", ResultStatus.Passed) }, null));

            var text = _subject.Format(feature1, feature2);
            const string expectedText = @"Feature: My feature

	Scenario: scenario1 - Passed
		Step 1: step1 - Passed

Feature: My feature2

	Scenario: scenario1 - Passed
		Step 1: step1 - Passed
";
            Assert.That(text, Is.EqualTo(expectedText));
        }
    }
}
