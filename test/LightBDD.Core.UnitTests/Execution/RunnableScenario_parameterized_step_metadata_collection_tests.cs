using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Formatting;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Expectations;
using LightBDD.Framework.Formatting;
using LightBDD.Framework.Parameters;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class RunnableScenario_parameterized_step_metadata_collection_tests : Steps
{
    private static readonly TestableScenarioFactory ScenarioFactory = TestableScenarioFactory.Create(cfg =>
        cfg.ConfigureNameFormatter(c => c.Use<UnderscoreToSpaceFormatter>()));

    [Test]
    public async Task It_should_capture_all_steps()
    {
        var scenario = await ScenarioFactory.RunScenario(r => r.Test().TestScenario(
            TestStep.CreateAsync(Given_step_with_parameter, "abc"),
            TestStep.CreateAsync(When_step_with_parameter, 123),
            TestStep.CreateAsync(Then_step_with_parameter, 3.15)));

        var steps = scenario.GetSteps();
        StepResultExpectation.AssertEqual(steps,
            new StepResultExpectation(1, 3, "GIVEN step with parameter \"abc\"", ExecutionStatus.Passed),
            new StepResultExpectation(2, 3, "WHEN step with parameter \"123\"", ExecutionStatus.Passed),
            new StepResultExpectation(3, 3, "THEN step with parameter \"3.15\"", ExecutionStatus.Passed)
        );
    }

    [Test]
    public async Task It_should_capture_steps_with_parameters_inserted_in_proper_places()
    {
        Task EntryMethod(ICoreScenarioStepsRunner r) => r.Test().TestScenario(
            TestStep.CreateAsync(Method_with_replaced_parameter_PARAM_in_name, "abc"),
            TestStep.CreateAsync(Method_with_inserted_parameter_param_in_name, "abc"),
            TestStep.CreateAsync(Method_with_appended_parameter_at_the_end_of_name, "abc"),
            TestStep.CreateAsync(ExtensionSteps.Extension_method_with_parameter_PARAM, "target", "abc"),
            TestStep.CreateAsync(Method_with_param1_param2_param3, "abc", "def", "123"),
            TestStep.CreateAsync(Method_with_appended_and_normal_param, "abc", "def", "123"));

        var scenario = await ScenarioFactory.RunScenario(EntryMethod);

        var steps = scenario.GetSteps();
        StepResultExpectation.AssertEqual(steps,
            new StepResultExpectation(1, 6, "Method with replaced parameter \"abc\" in name", ExecutionStatus.Passed),
            new StepResultExpectation(2, 6, "Method with inserted parameter param \"abc\" in name", ExecutionStatus.Passed),
            new StepResultExpectation(3, 6, "Method with appended parameter at the end of name [param: \"abc\"]", ExecutionStatus.Passed),
            new StepResultExpectation(4, 6, "Extension method with parameter \"abc\"", ExecutionStatus.Passed),
            new StepResultExpectation(5, 6, "Method with param1 \"def\" param2 \"123\" param3 \"abc\"", ExecutionStatus.Passed),
            new StepResultExpectation(6, 6, "Method with appended and normal param \"def\" [appended1: \"abc\"] [appended2: \"123\"]", ExecutionStatus.Passed)
        );
    }

    [Test]
    public async Task It_should_capture_steps_with_parameters()
    {
        var scenario = await ScenarioFactory.RunScenario(r => r.Test().TestScenario(
            TestStep.CreateAsync(Given_step_with_parameter, () => "abc"),
            TestStep.CreateAsync(Given_step_with_parameter, () => (string)null),
            TestStep.CreateAsync(When_step_with_parameter, () => 1),
            TestStep.CreateAsync(Then_step_with_parameter, () => 3.14)));

        var steps = scenario.GetSteps();
        StepResultExpectation.AssertEqual(steps,
            new StepResultExpectation(1, 4, "GIVEN step with parameter \"abc\"", ExecutionStatus.Passed),
            new StepResultExpectation(2, 4, "AND step with parameter \"<null>\"", ExecutionStatus.Passed),
            new StepResultExpectation(3, 4, "WHEN step with parameter \"1\"", ExecutionStatus.Passed),
            new StepResultExpectation(4, 4, "THEN step with parameter \"3.14\"", ExecutionStatus.Passed)
        );
    }

    [Test]
    public async Task It_should_capture_steps_with_parameters_and_failing_parameter_evaluation()
    {
        var scenario = await ScenarioFactory.RunScenario(r => r.Test().TestScenario(
                TestStep.CreateAsync(Given_step_with_parameter, () => "def"),
                TestStep.CreateAsync(When_step_with_parameter, ThrowingParameterInvocation),
                TestStep.CreateAsync(Then_step_with_parameter, () => 3.27)));

        var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
        Assert.That(ex.Message, Is.EqualTo(ParameterExceptionReason));

        var steps = scenario.GetSteps();
        StepResultExpectation.AssertEqual(steps,
            new StepResultExpectation(1, 3, "GIVEN step with parameter \"def\"", ExecutionStatus.Passed),
            new StepResultExpectation(2, 3, "WHEN step with parameter \"<?>\"", ExecutionStatus.Failed, $"Step 2 Failed: System.InvalidOperationException: {ParameterExceptionReason}"),
            new StepResultExpectation(3, 3, "THEN step with parameter \"<?>\"", ExecutionStatus.NotRun)
        );
    }

    [Test]
    public async Task It_should_capture_steps_with_parameters_and_failing_step()
    {
        var scenario = await ScenarioFactory.RunScenario(r => r.Test().TestScenario(
                TestStep.CreateAsync(Given_step_with_parameter, () => "abc"),
                TestStep.CreateAsync(When_step_with_parameter_throwing_exception, () => 5),
                TestStep.CreateAsync(Then_step_with_parameter, () => 3.2)));


        var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
        Assert.That(ex.Message, Is.EqualTo(ExceptionReason));

        var steps = scenario.GetSteps();
        StepResultExpectation.AssertEqual(steps,
            new StepResultExpectation(1, 3, "GIVEN step with parameter \"abc\"", ExecutionStatus.Passed),
            new StepResultExpectation(2, 3, "WHEN step with parameter \"5\" throwing exception", ExecutionStatus.Failed, $"Step 2 Failed: System.InvalidOperationException: {ExceptionReason}"),
            new StepResultExpectation(3, 3, "THEN step with parameter \"<?>\"", ExecutionStatus.NotRun)
        );
    }

    [Test]
    public async Task It_should_capture_constant_parameters_even_if_step_was_not_executed()
    {
        var scenario = await ScenarioFactory.RunScenario(r => r.Test().TestScenario(
                TestStep.CreateAsync(Given_step_with_parameter, () => "def"),
                TestStep.CreateAsync(When_step_with_parameter, ThrowingParameterInvocation),
                TestStep.CreateAsync(Then_step_with_parameter, 3.27)));


        var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
        Assert.That(ex.Message, Is.EqualTo(ParameterExceptionReason));

        var steps = scenario.GetSteps();
        StepResultExpectation.AssertEqual(steps,
            new StepResultExpectation(1, 3, "GIVEN step with parameter \"def\"", ExecutionStatus.Passed),
            new StepResultExpectation(2, 3, "WHEN step with parameter \"<?>\"", ExecutionStatus.Failed, $"Step 2 Failed: System.InvalidOperationException: {ParameterExceptionReason}"),
            new StepResultExpectation(3, 3, "THEN step with parameter \"3.27\"", ExecutionStatus.NotRun)
        );
    }

    [Test]
    public async Task It_should_capture_step_initialization_issues_in_scenario_execution_results()
    {
        var scenario = await ScenarioFactory.RunScenario(r => r.Test().TestScenario(GetFailingStepDescriptors("some reason")));
        Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
        Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario Failed: System.InvalidOperationException: Step group initialization failed: some reason"));
    }

    [Test]
    public async Task It_should_collect_results_for_scenarios_causing_formatting_failures()
    {
        var expectedErrorMessage = "Unable to format 'param' parameter of step '1/1 Method with wrong formatter param \"<?>\"': Input string was not in a correct format.";

        var scenario = await ScenarioFactory.RunScenario(r => r.Test().TestScenario(TestStep.CreateAsync(Method_with_wrong_formatter_param, () => "abc")));

        var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
        Assert.That(ex.Message, Is.EqualTo(expectedErrorMessage));

        Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
        Assert.That(scenario.StatusDetails, Is.EqualTo($"Step 1 Failed: System.InvalidOperationException: {expectedErrorMessage}"));

        StepResultExpectation.AssertEqual(scenario.GetSteps(),
            new StepResultExpectation(1, 1, "Method with wrong formatter param \"<?>\"", ExecutionStatus.Failed, $"Step 1 Failed: System.InvalidOperationException: {expectedErrorMessage}"));
    }

    [Test]
    public async Task It_should_capture_inline_complex_parameters_by_value_and_all_others_by_parameter_reference()
    {
        Task EntryMethod(object _, ICoreScenarioStepsRunner r) => r.Test().TestScenario(
            TestStep.CreateAsync(Method_with_inserted_parameter_param_in_name, () => Table.For(1, 2, 3)),
            TestStep.CreateAsync(Method_with_inserted_parameter_param_in_name, () => Table.ExpectData(1, 2, 3)),
            TestStep.CreateAsync(Method_with_inserted_parameter_param_in_name, () => Tree.For("abc")),
            TestStep.CreateAsync(Method_with_inserted_parameter_param_in_name, () => Tree.ExpectStrictMatch("abc")),
            TestStep.CreateAsync(Method_with_inserted_parameter_param_in_name, () => new Verifiable<string>(Expect.To.Equal("abc"))));

        var scenario = await ScenarioFactory.CreateBuilder()
            .WithScenarioEntryMethod(EntryMethod)
            .WithScenarioDecorators(new[] { new MultiAssertAttribute() })
            .Build()
            .RunAsync();


        scenario.ExecutionException.ShouldBeOfType<AggregateException>();
        var stepNames = scenario.GetSteps().Select(s => s.Info.Name.ToString()).ToArray();

        Assert.That(stepNames, Is.EqualTo(new[]
        {
            "Method with inserted parameter param \"<$param>\" in name",
            "Method with inserted parameter param \"<$param>\" in name",
            "Method with inserted parameter param \"<$param>\" in name",
            "Method with inserted parameter param \"<$param>\" in name",
            "Method with inserted parameter param \"expected: equals 'abc'\" in name"
        }));
    }

    public IEnumerable<StepDescriptor> GetFailingStepDescriptors(string reason)
    {
        yield return new StepDescriptor("test", (o, a) => Task.FromResult(DefaultStepResultDescriptor.Instance));
        throw new ArgumentException(reason);
    }

    private void Method_with_appended_parameter_at_the_end_of_name(object param) { }
    private void Method_with_inserted_parameter_param_in_name(object param) { }
    private void Method_with_replaced_parameter_PARAM_in_name(object param) { }
    private void Method_with_wrong_formatter_param([Format("{0")] object param) { }
    private void Method_with_appended_and_normal_param(object appended1, object param, object appended2) { }
    private void Method_with_param1_param2_param3(object param3, object param1, object param2) { }

    public class UnderscoreToSpaceFormatter : INameFormatter
    {
        public string FormatName(string name) => name.Replace('_', ' ');
    }
}

static class ExtensionSteps
{
    public static void Extension_method_with_parameter_PARAM(this object target, object param) { }
}