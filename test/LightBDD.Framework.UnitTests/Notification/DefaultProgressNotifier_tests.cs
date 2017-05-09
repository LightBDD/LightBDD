using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Framework.Notification;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using RandomTestValues;

namespace LightBDD.Framework.UnitTests.Notification
{
    [TestFixture]
    public class DefaultProgressNotifier_tests
    {
        private Queue<string> _captured;
        private DefaultProgressNotifier _notifier;

        private void Notify(string message)
        {
            _captured.Enqueue(message);
        }

        [SetUp]
        public void SetUp()
        {
            _captured = new Queue<string>();
            _notifier = new DefaultProgressNotifier(Notify);
        }

        [Test]
        public void It_should_capture_meaningful_information()
        {
            var featureInfo = RandomValue.Object<TestResults.TestFeatureInfo>();
            var scenarioInfo = RandomValue.Object<TestResults.TestScenarioInfo>();
            var stepInfo = RandomValue.Object<TestResults.TestStepInfo>();
            var stepResult = RandomValue.Object<TestResults.TestStepResult>();
            var scenarioResult = RandomValue.Object<TestResults.TestScenarioResult>();
            scenarioResult.Status = ExecutionStatus.Passed;

            var featureResult = RandomValue.Object<TestResults.TestFeatureResult>();
            var comment = RandomValue.String();

            var featureNotifier = (IFeatureProgressNotifier)_notifier;
            var scenarioNotifier = (IScenarioProgressNotifier)_notifier;

            featureNotifier.NotifyFeatureStart(featureInfo);
            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyStepStart(stepInfo);
            scenarioNotifier.NotifyStepComment(stepInfo, comment);
            scenarioNotifier.NotifyStepFinished(stepResult);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult);
            featureNotifier.NotifyFeatureFinished(featureResult);

            var expected = new[]
            {
                $"FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}\n  {featureInfo.Description}",
                $"SCENARIO: [{string.Join("][", scenarioInfo.Labels)}] {scenarioInfo.Name}",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: {stepInfo.Name}...",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: /* {comment} */",
                $"  STEP {stepResult.Info.GroupPrefix}{stepResult.Info.Number}/{stepResult.Info.GroupPrefix}{stepResult.Info.Total}: {stepResult.Info.Name} ({stepResult.Status} after {stepResult.ExecutionTime.Duration.FormatPretty()})",
                $"  SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}\n    {scenarioResult.StatusDetails}",
                $"FEATURE FINISHED: {featureResult.Info.Name}"
            };

            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_description_if_not_provided()
        {
            var featureInfo = RandomValue.Object<TestResults.TestFeatureInfo>();
            featureInfo.Description = null;
            ((IFeatureProgressNotifier)_notifier).NotifyFeatureStart(featureInfo);

            Assert.That(_captured.Single(), Is.EqualTo($"FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}"));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_labels_if_not_provided()
        {
            var featureInfo = RandomValue.Object<TestResults.TestFeatureInfo>();
            featureInfo.Labels = new string[0];
            ((IFeatureProgressNotifier)_notifier).NotifyFeatureStart(featureInfo);

            var expected = $"FEATURE: {featureInfo.Name}\n  {featureInfo.Description}";
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioStart_should_omit_labels_if_not_provided()
        {
            var scenarioInfo = RandomValue.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = new string[0];
            ((IScenarioProgressNotifier)_notifier).NotifyScenarioStart(scenarioInfo);

            var expected = $"SCENARIO: {scenarioInfo.Name}";
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_execution_time_if_not_provided()
        {
            var scenarioInfo = RandomValue.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = new string[0];
            var scenarioResult = RandomValue.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.ExecutionTime = null;

            var scenarioNotifier = (IScenarioProgressNotifier)_notifier;
            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult);

            var expected = new[]
            {
                $"SCENARIO: {scenarioInfo.Name}",
                $"  SCENARIO RESULT: {scenarioResult.Status}\n    {scenarioResult.StatusDetails}"
            };
            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_status_details_if_not_provided()
        {
            var scenarioInfo = RandomValue.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = new string[0];
            var scenarioResult = RandomValue.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.StatusDetails = null;

            var scenarioNotifier = (IScenarioProgressNotifier)_notifier;
            scenarioNotifier.NotifyScenarioStart(scenarioInfo);
            scenarioNotifier.NotifyScenarioFinished(scenarioResult);

            var expected = new[]
            {
                $"SCENARIO: {scenarioInfo.Name}",
                $"  SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}"
            };
            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

    }
}