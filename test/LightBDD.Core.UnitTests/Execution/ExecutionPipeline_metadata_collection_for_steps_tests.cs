using System.Threading.Tasks;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class ExecutionPipeline_metadata_collection_for_steps_tests
{
    class MyFeature : Steps
    {
        [TestScenario]
        public Task PassingSteps() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two,
            Then_step_three);

        [TestScenario]
        public Task FailingSteps() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_throwing_exception,
            Then_step_three);

        [TestScenario]
        public Task BypassedSteps() => TestScenarioBuilder.Current.TestScenario(
            Given_step_one,
            When_step_two_is_bypassed,
            Then_step_three);

        [TestScenario]
        public Task DefaultStepTypes() => TestScenarioBuilder.Current.TestScenario(
            TestStep.CreateNamed("Setup before steps", Some_step),
            TestStep.CreateNamed("Setup before steps", Some_step),
            TestStep.CreateNamed("Given step one", Some_step),
            TestStep.CreateNamed("Given step one", Some_step),
            TestStep.CreateNamed("When step two", Some_step),
            TestStep.CreateNamed("When step two", Some_step),
            TestStep.CreateNamed("Then step three", Some_step),
            TestStep.CreateNamed("Then step four", Some_step),
            TestStep.CreateNamed("Then step five", Some_step),
            TestStep.CreateNamed("Some step", Some_step));

        [TestScenario]
        public Task PredefinedStepTypes() => TestScenarioBuilder.Current.TestScenario(
            TestStep.CreateWithTypeAsync("setup", Some_step),
            TestStep.CreateWithTypeAsync("setup", Some_step),
            TestStep.CreateWithTypeAsync("given", Some_step),
            TestStep.CreateWithTypeAsync("given", Some_step),
            TestStep.CreateWithTypeAsync("when", Some_step),
            TestStep.CreateWithTypeAsync("when", Some_step),
            TestStep.CreateWithTypeAsync("then", Some_step),
            TestStep.CreateWithTypeAsync("then", Some_step),
            TestStep.CreateWithTypeAsync("something else", Some_step),
            TestStep.CreateWithTypeAsync("something else", Some_step));
    }

    [Test]
    public async Task It_should_capture_all_steps()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.PassingSteps());

        StepResultExpectation.AssertEqual(scenario.GetSteps(),
                new StepResultExpectation(1, 3, nameof(Steps.Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, nameof(Steps.When_step_two), ExecutionStatus.Passed),
                new StepResultExpectation(3, 3, nameof(Steps.Then_step_three), ExecutionStatus.Passed)
            );
    }

    [Test]
    public async Task It_should_capture_failed_steps()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.FailingSteps());

        StepResultExpectation.AssertEqual(scenario.GetSteps(),
                new StepResultExpectation(1, 3, nameof(Steps.Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, nameof(Steps.When_step_two_throwing_exception), ExecutionStatus.Failed, $"Step 2: System.InvalidOperationException: {Steps.ExceptionReason}"),
                new StepResultExpectation(3, 3, nameof(Steps.Then_step_three), ExecutionStatus.NotRun)
            );
    }

    [Test]
    public async Task It_should_capture_bypassed_steps()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.BypassedSteps());

        StepResultExpectation.AssertEqual(scenario.GetSteps(),
                new StepResultExpectation(1, 3, nameof(Steps.Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation(2, 3, nameof(Steps.When_step_two_is_bypassed), ExecutionStatus.Bypassed, $"Step 2: {Steps.BypassReason}"),
                new StepResultExpectation(3, 3, nameof(Steps.Then_step_three), ExecutionStatus.Passed)
            );
    }

    [Test]
    public async Task It_should_infer_and_capture_default_step_types()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.DefaultStepTypes());

        StepResultExpectation.AssertEqual(scenario.GetSteps(),
                new StepResultExpectation(1, 10, "SETUP before steps", ExecutionStatus.Passed),
                new StepResultExpectation(2, 10, "AND before steps", ExecutionStatus.Passed),
                new StepResultExpectation(3, 10, "GIVEN step one", ExecutionStatus.Passed),
                new StepResultExpectation(4, 10, "AND step one", ExecutionStatus.Passed),
                new StepResultExpectation(5, 10, "WHEN step two", ExecutionStatus.Passed),
                new StepResultExpectation(6, 10, "AND step two", ExecutionStatus.Passed),
                new StepResultExpectation(7, 10, "THEN step three", ExecutionStatus.Passed),
                new StepResultExpectation(8, 10, "AND step four", ExecutionStatus.Passed),
                new StepResultExpectation(9, 10, "AND step five", ExecutionStatus.Passed),
                new StepResultExpectation(10, 10, "Some step", ExecutionStatus.Passed)
            );
    }

    [Test]
    public async Task It_should_capture_predefined_step_types()
    {
        var scenario = await TestableCoreExecutionPipeline.Default.ExecuteScenario<MyFeature>(f => f.PredefinedStepTypes());

        StepResultExpectation.AssertEqual(scenario.GetSteps(),
            new StepResultExpectation(1, 10, "SETUP Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(2, 10, "AND Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(3, 10, "GIVEN Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(4, 10, "AND Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(5, 10, "WHEN Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(6, 10, "AND Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(7, 10, "THEN Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(8, 10, "AND Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(9, 10, "SOMETHING ELSE Some_step", ExecutionStatus.Passed),
            new StepResultExpectation(10, 10, "AND Some_step", ExecutionStatus.Passed)
            );
    }
}