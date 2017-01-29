using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Notification;
using LightBDD.UnitTests.Helpers.TestableIntegration;
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

            var feature = new TestableFeatureBddRunnerFactory(progressNotifier, fixture => progressNotifier).GetRunnerFor(GetType());
            var runner = feature.GetRunner(this);
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
                "Step Finish: 3/3 THEN step three should throw exception | Status:Failed | ExecutionTimePresent:True | Details:exception reason",
                "Scenario Finish: It should notify execution progress [lab1, lab2] <category 1, category 2> | Status:Failed | ExecutionTimePresent:True | Steps:3 | Details:Step 3: exception reason",
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

            var feature = new TestableFeatureBddRunnerFactory(progressNotifier, fixture => progressNotifier).GetRunnerFor(GetType());
            var runner = feature.GetRunner(this);
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
        public void It_should_instantiate_new_scenario_progress_notifier_for_each_scenario()
        {
            var notifiers = new List<CapturingProgressNotifier>();

            Func<object, IScenarioProgressNotifier> captureNotifierCreation = fixture =>
            {
                var notifier = new CapturingProgressNotifier();
                notifiers.Add(notifier);
                return notifier;
            };

            var runner = new TestableFeatureBddRunnerFactory(NoProgressNotifier.Default, captureNotifierCreation).GetRunnerFor(GetType()).GetRunner(this);

            runner.Test().TestNamedScenario("scenario1", TestStep.CreateSync(Given_step_one));
            runner.Test().TestNamedScenario("scenario2", TestStep.CreateSync(Given_step_one));
            runner.Test().TestNamedScenario("scenario3", TestStep.CreateSync(Given_step_one));

            Assert.That(notifiers.Count, Is.EqualTo(3));
            Assert.That(notifiers[0].Notifications.Count(n => n.StartsWith("Scenario Start: scenario1")), Is.EqualTo(1), "scenario1");
            Assert.That(notifiers[1].Notifications.Count(n => n.StartsWith("Scenario Start: scenario2")), Is.EqualTo(1), "scenario2");
            Assert.That(notifiers[2].Notifications.Count(n => n.StartsWith("Scenario Start: scenario3")), Is.EqualTo(1), "scenario3");
        }

        class CapturingProgressNotifier : IScenarioProgressNotifier, IFeatureProgressNotifier
        {
            private readonly List<string> _notifications = new List<string>();

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
                _notifications.Add($"Step {step.Number}/{step.Total} Comment: {comment}");
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
                return $"{step.Number}/{step.Total} {step.Name}";
            }
        }
    }
}