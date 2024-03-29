using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Notification;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using Moq;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [FeatureDescription("feature description")]
    [Label("label1")]
    [Label("label2")]
    [ScenarioCategory("category 1")]
    [TestFixture]
    public class CoreBddRunner_progress_notification_tests : Steps
    {
        [Test]
        [ScenarioCategory("category 2")]
        [Label("lab1")]
        [Label("lab2")]
        public void It_should_notify_execution_progress()
        {
            var progressNotifier = new CapturingProgressNotifier();

            var feature = new TestableFeatureRunnerRepository(progressNotifier).GetRunnerFor(GetType());
            var runner = feature.GetBddRunner(this);
            try
            {
                runner.Test().TestScenario(
                    Given_step_one,
                    When_step_two_with_comment,
                    Then_step_three_should_throw_exception);
            }
            catch { }
            feature.Dispose();

            string[] expected =
            {
                "Feature Start: CoreBddRunner progress notification tests [label1, label2]: feature description",
                "Scenario Start: It should notify execution progress [lab1, lab2] <category 1, category 2>",
                "Step Start: 1/3 GIVEN step one",
                "Step Finish: 1/3 GIVEN step one | Status:Passed | ExecutionTimePresent:True | Details:",
                "Step Start: 2/3 WHEN step two with comment",
                "Step 2/3 Comment: some comment",
                "Step Finish: 2/3 WHEN step two with comment | Status:Passed | ExecutionTimePresent:True | Details:",
                "Step Start: 3/3 THEN step three should throw exception",
                "Step Finish: 3/3 THEN step three should throw exception | Status:Failed | ExecutionTimePresent:True | Details:Step 3: exception reason",
                "Scenario Finish: It should notify execution progress [lab1, lab2] <category 1, category 2> | Status:Failed | ExecutionTimePresent:True | Steps:3 | Details:Step 3: exception reason",
                "Feature Finish: CoreBddRunner progress notification tests [label1, label2]: feature description | Scenarios:1"
            };
            Assert.That(progressNotifier.Notifications, Is.EqualTo(expected), "Expected:\r\n{0}\r\n\r\nGot:\r\n{1}\r\n\r\n", string.Join("\r\n", expected), string.Join("\r\n", progressNotifier.Notifications));
        }

        [Test]
        public void It_should_notify_attachments()
        {
            var progressNotifier = new CapturingProgressNotifier();

            var feature = new TestableFeatureRunnerRepository(progressNotifier).GetRunnerFor(GetType());
            var runner = feature.GetBddRunner(this);
            try
            {
                runner.Test().TestScenario(
                    Given_step_one,
                    When_step_two_with_attachment,
                    Then_step_three);
            }
            catch { }
            feature.Dispose();

            string[] expected =
            {
                "Feature Start: CoreBddRunner progress notification tests [label1, label2]: feature description",
                "Scenario Start: It should notify attachments [] <category 1>",
                "Step Start: 1/3 GIVEN step one",
                "Step Finish: 1/3 GIVEN step one | Status:Passed | ExecutionTimePresent:True | Details:",
                "Step Start: 2/3 WHEN step two with attachment",
                "Step 2/3 File Attachment - attachment1: attachment1.txt",
                "Step Finish: 2/3 WHEN step two with attachment | Status:Passed | ExecutionTimePresent:True | Details:",
                "Step Start: 3/3 THEN step three",
                "Step Finish: 3/3 THEN step three | Status:Passed | ExecutionTimePresent:True | Details:",
                "Scenario Finish: It should notify attachments [] <category 1> | Status:Passed | ExecutionTimePresent:True | Steps:3 | Details:",
                "Feature Finish: CoreBddRunner progress notification tests [label1, label2]: feature description | Scenarios:1"
            };
            Assert.That(progressNotifier.Notifications, Is.EqualTo(expected), "Expected:\r\n{0}\r\n\r\nGot:\r\n{1}\r\n\r\n", string.Join("\r\n", expected), string.Join("\r\n", progressNotifier.Notifications));
        }

        [Test]
        [ScenarioCategory("category 2")]
        [Label("lab1")]
        [Label("lab2")]
        public void It_should_notify_execution_progress_of_composite_steps()
        {
            var progressNotifier = new CapturingProgressNotifier();

            var feature = new TestableFeatureRunnerRepository(progressNotifier).GetRunnerFor(GetType());
            var runner = feature.GetBddRunner(this);
            try
            {
                runner.Test().TestGroupScenario(Composite_group);
            }
            catch { }
            feature.Dispose();

            string[] expected =
            {
                "Feature Start: CoreBddRunner progress notification tests [label1, label2]: feature description",
                "Scenario Start: It should notify execution progress of composite steps [lab1, lab2] <category 1, category 2>",
                "Step Start: 1/1 Composite group",
                "Step Start: 1.1/1.2 Passing step group with comment",

                "Step Start: 1.1.1/1.1.3 GIVEN step one",
                "Step Finish: 1.1.1/1.1.3 GIVEN step one | Status:Passed | ExecutionTimePresent:True | Details:",

                "Step Start: 1.1.2/1.1.3 WHEN step two with comment",
                "Step 1.1.2/1.1.3 Comment: some comment",
                "Step Finish: 1.1.2/1.1.3 WHEN step two with comment | Status:Passed | ExecutionTimePresent:True | Details:",

                "Step Start: 1.1.3/1.1.3 THEN step three",
                "Step Finish: 1.1.3/1.1.3 THEN step three | Status:Passed | ExecutionTimePresent:True | Details:",

                "Step Finish: 1.1/1.2 Passing step group with comment | Status:Passed | ExecutionTimePresent:True | Details:",

                "Step Start: 1.2/1.2 Bypassed step group",

                "Step Start: 1.2.1/1.2.3 GIVEN step one",
                "Step Finish: 1.2.1/1.2.3 GIVEN step one | Status:Passed | ExecutionTimePresent:True | Details:",

                "Step Start: 1.2.2/1.2.3 WHEN step two is bypassed",
                "Step Finish: 1.2.2/1.2.3 WHEN step two is bypassed | Status:Bypassed | ExecutionTimePresent:True | Details:Step 1.2.2: bypass reason",

                "Step Start: 1.2.3/1.2.3 THEN step three",
                "Step Finish: 1.2.3/1.2.3 THEN step three | Status:Passed | ExecutionTimePresent:True | Details:",

                "Step Finish: 1.2/1.2 Bypassed step group | Status:Bypassed | ExecutionTimePresent:True | Details:Step 1.2.2: bypass reason",

                "Step Finish: 1/1 Composite group | Status:Bypassed | ExecutionTimePresent:True | Details:Step 1.2.2: bypass reason",

                "Scenario Finish: It should notify execution progress of composite steps [lab1, lab2] <category 1, category 2> | Status:Bypassed | ExecutionTimePresent:True | Steps:1 | Details:Step 1.2.2: bypass reason",
                "Feature Finish: CoreBddRunner progress notification tests [label1, label2]: feature description | Scenarios:1"
            };
            Assert.That(progressNotifier.Notifications, Is.EqualTo(expected), "Expected:\r\n{0}\r\n\r\nGot:\r\n{1}\r\n\r\n", string.Join("\r\n", expected), string.Join("\r\n", progressNotifier.Notifications));
        }

        [Test]
        [ScenarioCategory("category 2")]
        [Label("lab1")]
        [Label("lab2")]
        public void It_should_notify_execution_progress_for_parameterized_steps()
        {
            var progressNotifier = new CapturingProgressNotifier();

            var feature = new TestableFeatureRunnerRepository(progressNotifier).GetRunnerFor(GetType());
            var runner = feature.GetBddRunner(this);
            try
            {
                runner.Test().TestScenario(
                    TestStep.CreateAsync(Given_step_with_parameter, () => "abc"),
                    TestStep.CreateAsync(When_step_with_parameter, ThrowingParameterInvocation),
                    TestStep.CreateAsync(Then_step_with_parameter, () => 2.22));
            }
            catch
            {
            }
            feature.Dispose();

            string[] expected =
            {
                "Feature Start: CoreBddRunner progress notification tests [label1, label2]: feature description",
                "Scenario Start: It should notify execution progress for parameterized steps [lab1, lab2] <category 1, category 2>",
                "Step Start: 1/3 GIVEN step with parameter \"abc\"",
                "Step Finish: 1/3 GIVEN step with parameter \"abc\" | Status:Passed | ExecutionTimePresent:True | Details:",
                "Scenario Finish: It should notify execution progress for parameterized steps [lab1, lab2] <category 1, category 2> | Status:Failed | ExecutionTimePresent:True | Steps:3 | Details:Step 2: parameter exception",
                "Feature Finish: CoreBddRunner progress notification tests [label1, label2]: feature description | Scenarios:1"
            };
            Assert.That(progressNotifier.Notifications, Is.EqualTo(expected), "Expected:\r\n{0}\r\n\r\nGot:\r\n{1}\r\n\r\n", string.Join("\r\n", expected), string.Join("\r\n", progressNotifier.Notifications));
        }

        [Test]
        [Obsolete]
        public void It_should_instantiate_new_scenario_progress_notifier_for_each_scenario()
        {
            var notifiers = new List<CapturingProgressNotifier>();

            Func<object, IScenarioProgressNotifier> captureNotifierCreation = fixture =>
            {
                var notifier = new CapturingProgressNotifier();
                notifiers.Add(notifier);
                return notifier;
            };

            var runner = new TestableFeatureRunnerRepository(NoProgressNotifier.Default, captureNotifierCreation).GetRunnerFor(GetType()).GetBddRunner(this);

            runner.Test().TestNamedScenario("scenario1", TestStep.CreateSync(Given_step_one));
            runner.Test().TestNamedScenario("scenario2", TestStep.CreateSync(Given_step_one));
            runner.Test().TestNamedScenario("scenario3", TestStep.CreateSync(Given_step_one));

            Assert.That(notifiers.Count, Is.EqualTo(3));
            Assert.That(notifiers[0].Notifications.Count(n => n.StartsWith("Scenario Start: scenario1")), Is.EqualTo(1), "scenario1");
            Assert.That(notifiers[1].Notifications.Count(n => n.StartsWith("Scenario Start: scenario2")), Is.EqualTo(1), "scenario2");
            Assert.That(notifiers[2].Notifications.Count(n => n.StartsWith("Scenario Start: scenario3")), Is.EqualTo(1), "scenario3");
        }

        void CaptureEvent<TEvent>(Mock<IProgressNotifier> notifier, Action<TEvent> callback) where TEvent : ProgressEvent
        {
            notifier.Setup(x => x.Notify(It.IsAny<TEvent>()))
                .Callback((ProgressEvent x) => callback.Invoke((TEvent)x));
        }

        [Test]
        public void It_should_wire_steps_scenario_and_feature_infos()
        {
            IFeatureInfo featureInfo = null;
            IFeatureResult featureResult = null;
            IScenarioInfo scenarioInfo = null;
            IScenarioResult scenarioResult = null;
            var stepInfos = new List<IStepInfo>();
            var stepResults = new List<IStepResult>();
            var progressNotifier = new Mock<IProgressNotifier>();

            CaptureEvent(progressNotifier, (FeatureStarting x) => featureInfo = x.Feature);
            CaptureEvent(progressNotifier, (FeatureFinished x) => featureResult = x.Result);
            CaptureEvent(progressNotifier, (ScenarioStarting x) => scenarioInfo = x.Scenario);
            CaptureEvent(progressNotifier, (ScenarioFinished x) => scenarioResult = x.Result);
            CaptureEvent(progressNotifier, (StepStarting x) => stepInfos.Add(x.Step));
            CaptureEvent(progressNotifier, (StepFinished x) => stepResults.Add(x.Result));

            var feature = new TestableFeatureRunnerRepository(progressNotifier.Object)
                .GetRunnerFor(GetType());
            var runner = feature.GetBddRunner(this);
            try
            {
                runner.Test().TestGroupScenario(Composite_group);
            }
            catch { }
            feature.Dispose();

            Assert.That(featureInfo, Is.Not.Null);
            Assert.That(featureInfo.RuntimeId, Is.Not.EqualTo(Guid.Empty), "Feature should have unique RuntimeId");
            Assert.That(featureResult.Info, Is.SameAs(featureInfo));

            Assert.That(scenarioInfo, Is.Not.Null);
            Assert.That(scenarioInfo.RuntimeId, Is.Not.EqualTo(Guid.Empty), "Scenario should have unique RuntimeId");
            Assert.That(scenarioResult.Info, Is.SameAs(scenarioInfo));
            Assert.That(scenarioInfo.Parent, Is.SameAs(featureInfo));

            Assert.That(stepInfos.Select(x => x.RuntimeId).Distinct().Count(), Is.EqualTo(9), "Each step should have unique RuntimeId");
            Assert.That(stepInfos[0].Parent, Is.SameAs(scenarioInfo));
            Assert.That(stepInfos[1].Parent, Is.SameAs(stepInfos[0]));
            Assert.That(stepInfos[2].Parent, Is.SameAs(stepInfos[1]));
            Assert.That(stepInfos[3].Parent, Is.SameAs(stepInfos[1]));
            Assert.That(stepInfos[4].Parent, Is.SameAs(stepInfos[1]));
            Assert.That(stepInfos[5].Parent, Is.SameAs(stepInfos[0]));
            Assert.That(stepInfos[6].Parent, Is.SameAs(stepInfos[5]));
            Assert.That(stepInfos[7].Parent, Is.SameAs(stepInfos[5]));
            Assert.That(stepInfos[8].Parent, Is.SameAs(stepInfos[5]));

            Assert.That(stepResults[0].Info, Is.SameAs(stepInfos[2]), "1.1.1");
            Assert.That(stepResults[1].Info, Is.SameAs(stepInfos[3]), "1.1.2");
            Assert.That(stepResults[2].Info, Is.SameAs(stepInfos[4]), "1.1.3");
            Assert.That(stepResults[3].Info, Is.SameAs(stepInfos[1]), "1.1");
            Assert.That(stepResults[4].Info, Is.SameAs(stepInfos[6]), "1.2.1");
            Assert.That(stepResults[5].Info, Is.SameAs(stepInfos[7]), "1.2.2");
            Assert.That(stepResults[6].Info, Is.SameAs(stepInfos[8]), "1.2.3");
            Assert.That(stepResults[7].Info, Is.SameAs(stepInfos[5]), "1.2");
            Assert.That(stepResults[8].Info, Is.SameAs(stepInfos[0]), "1");
        }

#pragma warning disable 618
        private class CapturingProgressNotifier : IProgressNotifier, IScenarioProgressNotifier, IFeatureProgressNotifier
#pragma warning restore 618
        {
            private readonly List<string> _notifications = new();

            public IEnumerable<string> Notifications => _notifications;

            public void NotifyFeatureStart(IFeatureInfo feature)
            {
                _notifications.Add($"Feature Start: {FormatFeature(feature)}");
            }

            public void NotifyFeatureFinished(IFeatureResult feature)
            {
                _notifications.Add($"Feature Finish: {FormatFeature(feature.Info)} | Scenarios:{feature.GetScenarios().Count()}");
            }

            public void NotifyScenarioStart(IScenarioInfo scenario)
            {
                _notifications.Add($"Scenario Start: {FormatScenario(scenario)}");
            }

            public void NotifyScenarioFinished(IScenarioResult scenario)
            {
                _notifications.Add($"Scenario Finish: {FormatScenario(scenario.Info)} | Status:{scenario.Status} | ExecutionTimePresent:{scenario.ExecutionTime != null} | Steps:{scenario.GetSteps().Count()} | Details:{scenario.StatusDetails}");
            }

            public void NotifyStepStart(IStepInfo step)
            {
                _notifications.Add($"Step Start: {FormatStep(step)}");
            }

            public void NotifyStepFinished(IStepResult step)
            {
                _notifications.Add($"Step Finish: {FormatStep(step.Info)} | Status:{step.Status} | ExecutionTimePresent:{step.ExecutionTime != null} | Details:{step.StatusDetails}");
            }

            public void NotifyStepComment(IStepInfo step, string comment)
            {
                _notifications.Add($"Step {step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total} Comment: {comment}");
            }

            private string FormatFeature(IFeatureInfo feature)
            {
                return $"{feature.Name} [{string.Join(", ", feature.Labels)}]: {feature.Description}";
            }

            private string FormatScenario(IScenarioInfo scenario)
            {
                return $"{scenario.Name} [{string.Join(", ", scenario.Labels)}] <{string.Join(", ", scenario.Categories)}>";
            }

            private string FormatStep(IStepInfo step)
            {
                return $"{step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total} {step.Name}";
            }

            public void Notify(ProgressEvent e)
            {
                switch (e)
                {
                    case FeatureFinished featureFinished:
                        NotifyFeatureFinished(featureFinished.Result);
                        break;
                    case FeatureStarting featureStarting:
                        NotifyFeatureStart(featureStarting.Feature);
                        break;
                    case ScenarioFinished scenarioFinished:
                        NotifyScenarioFinished(scenarioFinished.Result);
                        break;
                    case ScenarioStarting scenarioStarting:
                        NotifyScenarioStart(scenarioStarting.Scenario);
                        break;
                    case StepCommented stepCommented:
                        NotifyStepComment(stepCommented.Step, stepCommented.Comment);
                        break;
                    case StepFinished stepFinished:
                        NotifyStepFinished(stepFinished.Result);
                        break;
                    case StepStarting stepStarting:
                        NotifyStepStart(stepStarting.Step);
                        break;
                    case StepFileAttached stepFileAttached:
                        _notifications.Add($"Step {stepFileAttached.Step.GroupPrefix}{stepFileAttached.Step.Number}/{stepFileAttached.Step.GroupPrefix}{stepFileAttached.Step.Total} File Attachment - {stepFileAttached.Attachment.Name}: {stepFileAttached.Attachment.FilePath}");
                        break;
                }
            }
        }
    }
}