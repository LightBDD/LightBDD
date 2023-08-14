using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Metadata;
using LightBDD.Core.Notification;
using LightBDD.Core.Notification.Events;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class ExecutionPipeline_progress_notification_tests : Steps
{
    private readonly CapturingProgressNotifier _progressNotifier = new();
    private readonly TestableCoreExecutionPipeline _pipeline;

    public ExecutionPipeline_progress_notification_tests()
    {
        _pipeline = TestableCoreExecutionPipeline.Create(cfg => cfg.RegisterProgressNotifiers().Clear().Add(_progressNotifier));
    }

    class MyFeature : Steps
    {
        [TestScenario]
        public Task Scenario_with_comment_and_failed_step() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_with_comment,
            Then_step_three_should_throw_exception);

        [TestScenario]
        public Task Scenario_with_attachments() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_with_attachment,
            Then_step_three);

        [TestScenario]
        public Task Scenario_with_composite() => TestScenarioBuilder.Current.TestGroupScenario(Composite_group);

        [TestScenario]
        public Task Scenario_with_parameterized_steps() => TestScenarioBuilder.Current.TestScenario(
                TestStep.CreateAsync(Given_step_with_parameter, () => "abc"),
                TestStep.CreateAsync(When_step_with_parameter, ThrowingParameterInvocation),
                TestStep.CreateAsync(Then_step_with_parameter, () => 2.22));
    }

    [Test]
    public async Task It_should_notify_execution_progress()
    {
        await _pipeline.ExecuteScenario<MyFeature>(f => f.Scenario_with_comment_and_failed_step());

        AssertNotifications(
            "Test Run Start: LightBDD.Core.UnitTests",
            "Feature Start: MyFeature",
            "Scenario Start: Scenario_with_comment_and_failed_step",
            "Step Start: 1/3 Given_step_one",
            "Step Finish: 1/3 Given_step_one | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 2/3 When_step_two_with_comment",
            "Step 2/3 Comment: some comment",
            "Step Finish: 2/3 When_step_two_with_comment | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 3/3 Then_step_three_should_throw_exception",
            "Step Finish: 3/3 Then_step_three_should_throw_exception | Status:Failed | ExecutionTimePresent:True | Details:Step 3 Failed: System.InvalidOperationException: exception reason",
            "Scenario Finish: Scenario_with_comment_and_failed_step | Status:Failed | ExecutionTimePresent:True | Steps:3 | Details:Step 3 Failed: System.InvalidOperationException: exception reason",
            "Feature Finish: MyFeature | Scenarios:1",
            "Test Run Finish: LightBDD.Core.UnitTests | Status:Failed | Features:1");
    }

    [Test]
    public async Task It_should_notify_step_attachments()
    {
        await _pipeline.ExecuteScenario<MyFeature>(f => f.Scenario_with_attachments());

        AssertNotifications(
            "Test Run Start: LightBDD.Core.UnitTests",
            "Feature Start: MyFeature",
            "Scenario Start: Scenario_with_attachments",
            "Step Start: 1/3 Given_step_one",
            "Step Finish: 1/3 Given_step_one | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 2/3 When_step_two_with_attachment",
            "Step 2/3 File Attachment - attachment1: attachment1.txt",
            "Step Finish: 2/3 When_step_two_with_attachment | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 3/3 Then_step_three",
            "Step Finish: 3/3 Then_step_three | Status:Passed | ExecutionTimePresent:True | Details:",
            "Scenario Finish: Scenario_with_attachments | Status:Passed | ExecutionTimePresent:True | Steps:3 | Details:",
            "Feature Finish: MyFeature | Scenarios:1",
            "Test Run Finish: LightBDD.Core.UnitTests | Status:Passed | Features:1");
    }

    [Test]
    public async Task It_should_notify_execution_progress_of_composite_steps()
    {
        await _pipeline.ExecuteScenario<MyFeature>(f => f.Scenario_with_composite());

        AssertNotifications(
            "Test Run Start: LightBDD.Core.UnitTests",
            "Feature Start: MyFeature",
            "Scenario Start: Scenario_with_composite",
            "Step Start: 1/1 Composite_group",
            "Step Start: 1.1/1.2 Passing_step_group_with_comment",
            "Step Start: 1.1.1/1.1.3 Given_step_one",
            "Step Finish: 1.1.1/1.1.3 Given_step_one | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 1.1.2/1.1.3 When_step_two_with_comment",
            "Step 1.1.2/1.1.3 Comment: some comment",
            "Step Finish: 1.1.2/1.1.3 When_step_two_with_comment | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 1.1.3/1.1.3 Then_step_three",
            "Step Finish: 1.1.3/1.1.3 Then_step_three | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Finish: 1.1/1.2 Passing_step_group_with_comment | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 1.2/1.2 Bypassed_step_group",
            "Step Start: 1.2.1/1.2.3 Given_step_one",
            "Step Finish: 1.2.1/1.2.3 Given_step_one | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Start: 1.2.2/1.2.3 When_step_two_is_bypassed",
            "Step Finish: 1.2.2/1.2.3 When_step_two_is_bypassed | Status:Bypassed | ExecutionTimePresent:True | Details:Step 1.2.2 Bypassed: bypass reason",
            "Step Start: 1.2.3/1.2.3 Then_step_three",
            "Step Finish: 1.2.3/1.2.3 Then_step_three | Status:Passed | ExecutionTimePresent:True | Details:",
            "Step Finish: 1.2/1.2 Bypassed_step_group | Status:Bypassed | ExecutionTimePresent:True | Details:Step 1.2.2 Bypassed: bypass reason",
            "Step Finish: 1/1 Composite_group | Status:Bypassed | ExecutionTimePresent:True | Details:Step 1.2.2 Bypassed: bypass reason",
            "Scenario Finish: Scenario_with_composite | Status:Bypassed | ExecutionTimePresent:True | Steps:1 | Details:Step 1.2.2 Bypassed: bypass reason",
            "Feature Finish: MyFeature | Scenarios:1",
            "Test Run Finish: LightBDD.Core.UnitTests | Status:Passed | Features:1");
    }

    [Test]
    public async Task It_should_notify_execution_progress_for_parameterized_steps()
    {
        await _pipeline.ExecuteScenario<MyFeature>(f => f.Scenario_with_parameterized_steps());

        AssertNotifications(
            "Test Run Start: LightBDD.Core.UnitTests", "Feature Start: MyFeature",
            "Scenario Start: Scenario_with_parameterized_steps",
            "Step Start: 1/3 Given_step_with_parameter [parameter: \"abc\"]",
            "Step Finish: 1/3 Given_step_with_parameter [parameter: \"abc\"] | Status:Passed | ExecutionTimePresent:True | Details:",
            "Scenario Finish: Scenario_with_parameterized_steps | Status:Failed | ExecutionTimePresent:True | Steps:3 | Details:Step 2 Failed: System.InvalidOperationException: parameter exception",
            "Feature Finish: MyFeature | Scenarios:1",
            "Test Run Finish: LightBDD.Core.UnitTests | Status:Failed | Features:1");
    }

    [Test]
    public async Task It_should_wire_steps_scenario_and_feature_infos()
    {
        IFeatureInfo featureInfo = null;
        IFeatureResult featureResult = null;
        IScenarioInfo scenarioInfo = null;
        IScenarioResult scenarioResult = null;
        var stepInfos = new List<IStepInfo>();
        var stepResults = new List<IStepResult>();

        CaptureEvent((FeatureStarting x) => featureInfo = x.Feature);
        CaptureEvent((FeatureFinished x) => featureResult = x.Result);
        CaptureEvent((ScenarioStarting x) => scenarioInfo = x.Scenario);
        CaptureEvent((ScenarioFinished x) => scenarioResult = x.Result);
        CaptureEvent((StepStarting x) => stepInfos.Add(x.Step));
        CaptureEvent((StepFinished x) => stepResults.Add(x.Result));

        await _pipeline.ExecuteScenario<MyFeature>(f => f.Scenario_with_composite());

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

    void CaptureEvent<TEvent>(Action<TEvent> callback) where TEvent : ProgressEvent
    {
        _progressNotifier.OnNotify += e =>
        {
            if (e is TEvent ev)
                callback(ev);
        };
    }

    private void AssertNotifications(params string[] expected)
    {
        var actual = _progressNotifier.Notifications.ToArray();
        expected.ShouldBe(actual, $"Expected:\r\n{string.Join("\r\n", expected)}\r\n\r\nGot:\r\n{string.Join("\r\n", actual)}\r\n\r\n");
    }

    private class CapturingProgressNotifier : IProgressNotifier
    {
        private readonly List<string> _notifications = new();
        public IEnumerable<string> Notifications => _notifications;
        public event Action<ProgressEvent> OnNotify;

        public void Notify(ProgressEvent e)
        {
            var message = e switch
            {
                TestRunStarting ev => $"Test Run Start: {ev.TestRun.Name}",
                TestRunFinished ev => $"Test Run Finish: {ev.Result.Info.Name} | Status:{ev.Result.OverallStatus} | Features:{ev.Result.Features.Count}",
                FeatureFinished ev => $"Feature Finish: {FormatFeature(ev.Result.Info)} | Scenarios:{ev.Result.GetScenarios().Count()}",
                FeatureStarting ev => $"Feature Start: {FormatFeature(ev.Feature)}",
                ScenarioFinished ev => $"Scenario Finish: {FormatScenario(ev.Result.Info)} | Status:{ev.Result.Status} | ExecutionTimePresent:{ev.Result.ExecutionTime != ExecutionTime.None} | Steps:{ev.Result.GetSteps().Count()} | Details:{ev.Result.StatusDetails}",
                ScenarioStarting ev => $"Scenario Start: {FormatScenario(ev.Scenario)}",
                StepCommented ev => $"Step {FormatStepRef(ev.Step)} Comment: {ev.Comment}",
                StepFinished ev => $"Step Finish: {FormatStep(ev.Result.Info)} | Status:{ev.Result.Status} | ExecutionTimePresent:{ev.Result.ExecutionTime != null} | Details:{ev.Result.StatusDetails}",
                StepStarting ev => $"Step Start: {FormatStep(ev.Step)}",
                StepFileAttached ev => $"Step {FormatStepRef(ev.Step)} File Attachment - {ev.Attachment.Name}: {ev.Attachment.FilePath}",
                _ => $"{e.GetType().Name}"
            };
            _notifications.Add(message);
            OnNotify?.Invoke(e);
        }

        private string FormatFeature(IFeatureInfo feature) => $"{feature.Name}";
        private string FormatScenario(IScenarioInfo scenario) => $"{scenario.Name}";
        private string FormatStep(IStepInfo step) => $"{FormatStepRef(step)} {step.Name}";
        private string FormatStepRef(IStepInfo step) => $"{step.GroupPrefix}{step.Number}/{step.GroupPrefix}{step.Total}";
    }
}