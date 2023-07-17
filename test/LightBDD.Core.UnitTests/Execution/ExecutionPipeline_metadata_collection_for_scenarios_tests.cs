using System;
using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class ExecutionPipeline_metadata_collection_for_scenarios_tests
{
    class MyFeature : Steps
    {
        [TestScenario]
        public Task SomeScenario() => TestScenarioBuilder.Current.TestScenario(Some_step);

        [TestScenario]
        [Label("Ticket-1")]
        [Label("Ticket-2")]
        public Task SomeScenarioWithLabels() => TestScenarioBuilder.Current.TestScenario(Some_step);

        [TestScenario]
        [Label("Ticket-1")]
        [Label("Ticket-2")]
        [ScenarioCategory("catA")]
        [ScenarioCategory("catB")]
        public Task SomeScenarioWithLabelsAndCategories() => TestScenarioBuilder.Current.TestScenario(Some_step);

        [TestScenario]
        public Task PassingScenario() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two,
            Then_step_three);

        [TestScenario]
        public Task FailingScenario() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_throwing_exception,
            Then_step_three);

        [TestScenario]
        public Task BypassedScenario() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_is_bypassed,
            Then_step_three);

        [TestScenario]
        public Task IgnoredScenario() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_is_bypassed,
            Then_step_three_should_be_ignored,
            Then_step_four);

        [TestScenario]
        public Task BypassedThenFailedScenario() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_is_bypassed,
            Then_step_three_should_throw_exception);
    }

    [ScenarioCategory("global1")]
    [ScenarioCategory("global2")]
    class FeatureWithGlobalCategories : Steps
    {
        [TestScenario]
        public Task ScenarioWithNoCategories() => TestScenarioBuilder.Current.TestScenario(Some_step);


        [TestScenario]
        [ScenarioCategory("local1")]
        [ScenarioCategory("local2")]
        public Task ScenarioWithLocalCategories() => TestScenarioBuilder.Current.TestScenario(Some_step);
    }

    [Test]
    public async Task It_should_capture_scenario_name()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.SomeScenario());
        Assert.That(scenario.Info.Name.ToString(), Is.EqualTo(nameof(MyFeature.SomeScenario)));
        Assert.That(scenario.Info.Labels, Is.Empty);
        Assert.That(scenario.Info.Categories, Is.Empty);
    }

    [Test]
    public async Task It_should_capture_scenario_name_with_labels()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.SomeScenarioWithLabels());
        Assert.That(scenario.Info.Name.ToString(), Is.EqualTo(nameof(MyFeature.SomeScenarioWithLabels)));
        Assert.That(scenario.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
        Assert.That(scenario.Info.Categories, Is.Empty);
    }

    [Test]
    public async Task It_should_capture_scenario_name_with_categories()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.SomeScenarioWithLabelsAndCategories());
        Assert.That(scenario.Info.Name.ToString(), Is.EqualTo(nameof(MyFeature.SomeScenarioWithLabelsAndCategories)));
        Assert.That(scenario.Info.Labels, Is.EqualTo(new[] { "Ticket-1", "Ticket-2" }));
        Assert.That(scenario.Info.Categories, Is.EqualTo(new[] { "catA", "catB" }));
    }

    [Test]
    public async Task It_should_capture_scenario_execution_status_for_passing_steps()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.PassingScenario());
        Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Passed));
        Assert.Null(scenario.StatusDetails);
    }

    [Test]
    public async Task It_should_capture_scenario_execution_status_for_failing_steps()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.FailingScenario());
        Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
        Assert.That(scenario.StatusDetails, Is.EqualTo($"Step 2: {Steps.ExceptionReason}"));
    }

    [Test]
    public async Task It_should_capture_scenario_status_for_passing_steps_with_bypassed_one()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.BypassedScenario());
        Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Bypassed));
    }

    [Test]
    public async Task It_should_capture_scenario_status_for_ignored_steps()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.IgnoredScenario());
        Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Ignored));
    }

    [Test]
    public async Task It_should_capture_scenario_execution_status_details_from_all_steps()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.BypassedThenFailedScenario());
        Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
        var expected = $"Step 2: {Steps.BypassReason}{Environment.NewLine}Step 3: {Steps.ExceptionReason}";

        Assert.That(scenario.StatusDetails, Is.EqualTo(expected));
    }

    [Test]
    public async Task It_should_capture_scenario_name_with_global_categories()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<FeatureWithGlobalCategories>(f => f.ScenarioWithNoCategories());
        Assert.That(scenario.Info.Categories, Is.EqualTo(new[] { "global1", "global2" }));
    }

    [Test]
    [ScenarioCategory("local1")]
    [ScenarioCategory("local2")]
    public async Task It_should_capture_scenario_name_with_global_and_local_categories()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<FeatureWithGlobalCategories>(f => f.ScenarioWithLocalCategories());
        Assert.That(scenario.Info.Categories, Is.EqualTo(new[] { "global1", "global2", "local1", "local2" }));
    }
}