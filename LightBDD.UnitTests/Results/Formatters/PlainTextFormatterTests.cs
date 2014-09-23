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
            result.AddScenario(new ScenarioResult("name", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(new TimeSpan(0, 1, 1)), 
                new StepResult(2, "step2", ResultStatus.Ignored, "Not implemented yet").SetExecutionTime(new TimeSpan(0, 0, 0, 1, 100))
            }, "Label 2").SetExecutionTime(new TimeSpan(0, 0, 1, 2, 100)));
            result.AddScenario(new ScenarioResult("name2", new[]
            {
                new StepResult(1, "step3", ResultStatus.Passed).SetExecutionTime(new TimeSpan(0, 0, 0, 2, 107)), 
                new StepResult(2, "step4", ResultStatus.Failed, string.Format("  Expected: True{0}  But was: False", Environment.NewLine)).SetExecutionTime(new TimeSpan(0, 0, 0, 0, 50)),
                new StepResult(3, "step5", ResultStatus.NotRun)
            }, null).SetExecutionTime(new TimeSpan(0, 0, 0, 2, 157)));
            var text = _subject.Format(result);
            const string expectedText = @"Feature: [Label 1] My feature
	My feature
	long description

	Scenario: [Label 2] name - Ignored (1m 02s)
		Step 1: step1 - Passed (1m 01s)
		Step 2: step2 - Ignored (1s 100ms)

		Details: Not implemented yet

	Scenario: name2 - Failed (2s 157ms)
		Step 1: step3 - Passed (2s 107ms)
		Step 2: step4 - Failed (50ms)
		Step 3: step5 - NotRun

		Details: Expected: True
			  But was: False
";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Should_format_feature_without_description_nor_label_nor_details()
        {
            var result = new FeatureResult("My feature", null, null);
            result.AddScenario(new ScenarioResult("name", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20)), 
                new StepResult(2, "step2", ResultStatus.Ignored).SetExecutionTime(TimeSpan.FromMilliseconds(5))
            }, null).SetExecutionTime(TimeSpan.FromMilliseconds(25)));
            var text = _subject.Format(result);
            const string expectedText = @"Feature: My feature

	Scenario: name - Ignored (25ms)
		Step 1: step1 - Passed (20ms)
		Step 2: step2 - Ignored (5ms)
";
            Assert.That(text, Is.EqualTo(expectedText));
        }

        [Test]
        public void Multiple_features_should_be_separated_by_new_line()
        {
            var feature1 = new FeatureResult("My feature", null, null);
            feature1.AddScenario(new ScenarioResult("scenario1", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20))
            }, null).SetExecutionTime(TimeSpan.FromMilliseconds(20)));

            var feature2 = new FeatureResult("My feature2", null, null);
            feature2.AddScenario(new ScenarioResult("scenario1", new[]
            {
                new StepResult(1, "step1", ResultStatus.Passed).SetExecutionTime(TimeSpan.FromMilliseconds(20))
            }, null).SetExecutionTime(TimeSpan.FromMilliseconds(20)));

            var text = _subject.Format(feature1, feature2);
            const string expectedText = @"Feature: My feature

	Scenario: scenario1 - Passed (20ms)
		Step 1: step1 - Passed (20ms)

Feature: My feature2

	Scenario: scenario1 - Passed (20ms)
		Step 1: step1 - Passed (20ms)
";
            Assert.That(text, Is.EqualTo(expectedText));
        }
    }
}
