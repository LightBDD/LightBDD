﻿using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.Results.Parameters;
using LightBDD.Core.Results.Parameters.Tabular;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Parameters;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;

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
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            var stepInfo = Fake.Object<TestResults.TestStepInfo>();
            var stepResult = Fake.Object<TestResults.TestStepResult>();
            stepResult.Parameters = new IParameterResult[]
            {
                new TestResults.TestParameterResult("table",
                    TestResults.CreateTabularParameterDetails(ParameterVerificationStatus.Failure)
                        .WithKeyColumns("Key")
                        .WithValueColumns("Value1", "Value2")
                        .AddRow(TableRowType.Matching,
                            ParameterVerificationStatus.Success,
                            TestResults.CreateValueResult("1"),
                            TestResults.CreateValueResult("abc"),
                            TestResults.CreateValueResult("some value"))
                        .AddRow(TableRowType.Matching,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("2"),
                            TestResults.CreateValueResult("def"),
                            TestResults.CreateValueResult("value", "val", ParameterVerificationStatus.Failure))
                        .AddRow(TableRowType.Missing,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("3"),
                            TestResults.CreateValueResult("XXX", "<null>", ParameterVerificationStatus.NotProvided),
                            TestResults.CreateValueResult("YYY", "<null>", ParameterVerificationStatus.NotProvided))
                        .AddRow(TableRowType.Surplus,
                            ParameterVerificationStatus.Failure,
                            TestResults.CreateValueResult("4"),
                            TestResults.CreateValueResult("<null>", "XXX",
                                ParameterVerificationStatus.Failure),
                            TestResults.CreateValueResult("<null>", "YYY",
                                ParameterVerificationStatus.Failure))
                ),
                new TestResults.TestParameterResult("tree",CreateTreeParameterResult())
            };
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Status = ExecutionStatus.Passed;

            var featureResult = Fake.Object<TestResults.TestFeatureResult>();
            var comment = Fake.String();
            var attachment = new FileAttachment(Fake.String(), Fake.String(), Fake.String());

            var eventTime = new EventTime();
            _notifier.Notify(new FeatureStarting(eventTime, featureInfo));
            _notifier.Notify(new ScenarioStarting(eventTime, scenarioInfo));
            _notifier.Notify(new StepStarting(eventTime, stepInfo));
            _notifier.Notify(new StepCommented(eventTime, stepInfo, comment));
            _notifier.Notify(new StepFileAttached(eventTime, stepInfo, attachment));
            _notifier.Notify(new StepFinished(eventTime, stepResult));
            _notifier.Notify(new ScenarioFinished(eventTime, scenarioResult));
            _notifier.Notify(new FeatureFinished(eventTime, featureResult));

            var expectedTable = @"    table:
    +-+---+----------+----------+
    |#|Key|Value1    |Value2    |
    +-+---+----------+----------+
    |=|1  |abc       |some value|
    |!|2  |def       |val/value |
    |-|3  |<null>/XXX|<null>/YYY|
    |+|4  |XXX/<null>|YYY/<null>|
    +-+---+----------+----------+"
                .Replace("\r", "")
                .Replace("\n", Environment.NewLine);

            var expectedTree = @"    tree:
    = $: <object>
    = $.Items: <array:1>
    ! $.Items[0]: False/True
    = $.Name: Bob
    ! $.Surname: Johnson/<none>
    ! $.LastName: <none>/Johnson"
                .Replace("\r", "")
                .Replace("\n", Environment.NewLine);

            var expected = new[]
            {
                $"FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}{Environment.NewLine}  {featureInfo.Description}",
                $"SCENARIO: [{string.Join("][", scenarioInfo.Labels)}] {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: {stepInfo.Name}...",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: /* {comment} */",
                $"  STEP {stepInfo.GroupPrefix}{stepInfo.Number}/{stepInfo.GroupPrefix}{stepInfo.Total}: 🔗{attachment.Name}: {attachment.FilePath}",
                $"  STEP {stepResult.Info.GroupPrefix}{stepResult.Info.Number}/{stepResult.Info.GroupPrefix}{stepResult.Info.Total}: {stepResult.Info.Name} ({stepResult.Status} after {stepResult.ExecutionTime.Duration.FormatPretty()}){Environment.NewLine}{expectedTable}{Environment.NewLine}{expectedTree}",
                $"  SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}{Environment.NewLine}    {scenarioResult.StatusDetails}",
                $"FEATURE FINISHED: {featureResult.Info.Name}"
            };

            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

        private IParameterDetails CreateTreeParameterResult()
        {
            var expected = new
            {
                Name = "Bob",
                Surname = "Johnson",
                Items = new[] { false }
            };
            var actual = new
            {
                Name = "Bob",
                LastName = "Johnson",
                Items = new[] { true }
            };
            var tree = Tree.ExpectEquivalent(expected);
            tree.SetActual(actual);
            return tree.Details;
        }

        [Test]
        public void NotifyFeatureStart_should_omit_description_if_not_provided()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            featureInfo.Description = null;
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            Assert.That(_captured.Single(), Is.EqualTo($"FEATURE: [{string.Join("][", featureInfo.Labels)}] {featureInfo.Name}"));
        }

        [Test]
        public void NotifyFeatureStart_should_omit_labels_if_not_provided()
        {
            var featureInfo = Fake.Object<TestResults.TestFeatureInfo>();
            featureInfo.Labels = Array.Empty<string>();
            _notifier.Notify(new FeatureStarting(new EventTime(), featureInfo));

            var expected = $"FEATURE: {featureInfo.Name}{Environment.NewLine}  {featureInfo.Description}";
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioStart_should_omit_labels_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));

            var expected = $"SCENARIO: {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}";
            Assert.That(_captured.Single(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_execution_time_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.ExecutionTime = null;

            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));
            _notifier.Notify(new ScenarioFinished(new EventTime(), scenarioResult));

            var expected = new[]
            {
                $"SCENARIO: {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}",
                $"  SCENARIO RESULT: {scenarioResult.Status}{Environment.NewLine}    {scenarioResult.StatusDetails}"
            };
            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

        [Test]
        public void NotifyScenarioFinished_should_omit_status_details_if_not_provided()
        {
            var scenarioInfo = Fake.Object<TestResults.TestScenarioInfo>();
            scenarioInfo.Labels = Array.Empty<string>();
            var scenarioResult = Fake.Object<TestResults.TestScenarioResult>();
            scenarioResult.Info = scenarioInfo;
            scenarioResult.Status = ExecutionStatus.Passed;
            scenarioResult.StatusDetails = null;

            _notifier.Notify(new ScenarioStarting(new EventTime(), scenarioInfo));
            _notifier.Notify(new ScenarioFinished(new EventTime(), scenarioResult));

            var expected = new[]
            {
                $"SCENARIO: {scenarioInfo.Name}{Environment.NewLine}  {scenarioInfo.Description}",
                $"  SCENARIO RESULT: {scenarioResult.Status} after {scenarioResult.ExecutionTime.Duration.FormatPretty()}"
            };
            Assert.That(_captured.ToArray(), Is.EqualTo(expected));
        }

    }
}