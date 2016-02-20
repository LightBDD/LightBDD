using System.Collections.Generic;
using System.Linq;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Core.UnitTests.TestableIntegration;
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

            var runner = new TestableBddRunner(GetType(), progressNotifier);
            try
            {
                runner.TestScenario(
                    Given_step_one,
                    When_step_two_with_comment,
                    Then_step_three_should_throw_exception);
            }
            catch { }
            runner.Dispose();

            string[] expected =
            {
                "Feature Start: CoreBddRunner progress notification tests [label1, label2]: feature description",
                "Scenario Start: It should notify execution progress [lab1, lab2] <category 1, category 2>",
                "Step Start: 1/3 Given step one",
                "Step Finish: 1/3 Given step one | Status:Passed | ExecutionTimePresent:True | Details:",
                "Step Start: 2/3 When step two with comment",
                "Step Comment: some comment",
                "Step Finish: 2/3 When step two with comment | Status:Passed | ExecutionTimePresent:True | Details:",
                "Step Start: 3/3 Then step three should throw exception",
                "Step Finish: 3/3 Then step three should throw exception | Status:Failed | ExecutionTimePresent:True | Details:exception reason",
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

            var runner = new TestableBddRunner(GetType(), progressNotifier);
            try
            {
                runner.TestParameterizedScenario(
                    TestSyntax.ParameterizedWithFunction(Given_step_with_parameter, () => 1),
                    TestSyntax.ParameterizedWithFunction(When_step_with_parameter, ThrowingParameterInvocation),
                    TestSyntax.ParameterizedWithFunction(Then_step_with_parameter, () => 2));
            }
            catch
            {
            }
            runner.Dispose();

            string[] expected =
            {
                "Feature Start: CoreBddRunner progress notification tests [label1, label2]: feature description",
                "Scenario Start: It should notify execution progress for parameterized steps [lab1, lab2] <category 1, category 2>",
                "Step Start: 1/3 Given step with parameter \"1\"",
                "Step Finish: 1/3 Given step with parameter \"1\" | Status:Passed | ExecutionTimePresent:True | Details:",
                "Scenario Finish: It should notify execution progress for parameterized steps [lab1, lab2] <category 1, category 2> | Status:Failed | ExecutionTimePresent:True | Steps:3 | Details:Step 2: parameter exception",
                "Feature Finish: CoreBddRunner progress notification tests [label1, label2]: feature description | Scenarios:1"
            };
            Assert.That(progressNotifier.Notifications, Is.EqualTo(expected), "Expected:\r\n{0}\r\n\r\nGot:\r\n{1}\r\n\r\n", string.Join("\r\n", expected), string.Join("\r\n", progressNotifier.Notifications));
        }

        class CapturingProgressNotifier : IProgressNotifier
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

            public void NotifyStepComment(string comment)
            {
                _notifications.Add($"Step Comment: {comment}");
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