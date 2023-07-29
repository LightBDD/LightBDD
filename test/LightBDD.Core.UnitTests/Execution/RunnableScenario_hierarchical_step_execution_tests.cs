using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class RunnableScenario_hierarchical_step_execution_tests : Steps
    {
        [Test]
        public async Task Runner_should_capture_details_about_sub_step_initialization_failure()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Incorrect_step_group));

            StepResultExpectation.AssertEqual(scenario.GetSteps(),
                new StepResultExpectation(1, 1, nameof(Incorrect_step_group), ExecutionStatus.Failed, "Step 1 Failed: System.InvalidOperationException: Step group initialization failed: abc"));
        }

        private TestCompositeStep Incorrect_step_group()
        {
            IEnumerable<StepDescriptor> GetSteps()
            {
                Func<object> stepAction = GetSteps;
                yield return new StepDescriptor(stepAction.GetMethodInfo(), (ctx, args) => Task.FromResult(DefaultStepResultDescriptor.Instance));
                throw new Exception("abc");
            }

            return new TestCompositeStep(() => null, GetSteps());
        }

        [Test]
        public async Task Runner_should_execute_all_steps_within_group()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Passing_step_group));
            var steps = scenario.GetSteps().ToArray();

            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, nameof(Passing_step_group), ExecutionStatus.Passed));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, nameof(When_step_two), ExecutionStatus.Passed),
                new StepResultExpectation("1.", 3, 3, nameof(Then_step_three), ExecutionStatus.Passed)
            );
        }

        [Test]
        public async Task Runner_should_mark_step_failed_if_substep_fails()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Failing_step_group));

            var steps = scenario.GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, nameof(Failing_step_group), ExecutionStatus.Failed, $"Step 1.2 Failed: System.InvalidOperationException: {ExceptionReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, nameof(When_step_two_throwing_exception), ExecutionStatus.Failed, $"Step 1.2 Failed: System.InvalidOperationException: {ExceptionReason}"),
                new StepResultExpectation("1.", 3, 3, nameof(Then_step_three), ExecutionStatus.NotRun)
            );
        }

        [Test]
        public async Task Runner_should_mark_step_ignored_if_substep_is_ignored()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Ignored_step_group));

            var steps = scenario.GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, nameof(Ignored_step_group), ExecutionStatus.Ignored, $"Step 1.2 Ignored: {IgnoreReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, nameof(When_step_two_ignoring_scenario), ExecutionStatus.Ignored, $"Step 1.2 Ignored: {IgnoreReason}"),
                new StepResultExpectation("1.", 3, 3, nameof(Then_step_three), ExecutionStatus.NotRun)
            );
        }

        [Test]
        public async Task Runner_should_mark_step_bypassed_if_substep_is_bypassed()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Bypassed_step_group));

            var steps = scenario.GetSteps().ToArray();

            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 1, nameof(Bypassed_step_group), ExecutionStatus.Bypassed, $"Step 1.2 Bypassed: {BypassReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, nameof(When_step_two_is_bypassed), ExecutionStatus.Bypassed, $"Step 1.2 Bypassed: {BypassReason}"),
                new StepResultExpectation("1.", 3, 3, nameof(Then_step_three), ExecutionStatus.Passed)
            );
        }

        [Test]
        public async Task Runner_should_properly_associate_steps_to_the_group()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Passing_step_group, Composite_group));

            var steps = scenario.GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 2, nameof(Passing_step_group), ExecutionStatus.Passed),
                new StepResultExpectation(2, 2, nameof(Composite_group), ExecutionStatus.Bypassed, $"Step 2.2.2 Bypassed: {BypassReason}"));

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation("1.", 2, 3, nameof(When_step_two), ExecutionStatus.Passed),
                new StepResultExpectation("1.", 3, 3, nameof(Then_step_three), ExecutionStatus.Passed)
            );

            StepResultExpectation.AssertEqual(steps[1].GetSubSteps().ElementAt(0).GetSubSteps(),
                new StepResultExpectation("2.1.", 1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation("2.1.", 2, 3, nameof(When_step_two_with_comment), ExecutionStatus.Passed, null, CommentReason),
                new StepResultExpectation("2.1.", 3, 3, nameof(Then_step_three), ExecutionStatus.Passed)
            );
            StepResultExpectation.AssertEqual(steps[1].GetSubSteps().ElementAt(1).GetSubSteps(),
                new StepResultExpectation("2.2.", 1, 3, nameof(Given_step_one), ExecutionStatus.Passed),
                new StepResultExpectation("2.2.", 2, 3, nameof(When_step_two_is_bypassed), ExecutionStatus.Bypassed, $"Step 2.2.2 Bypassed: {BypassReason}"),
                new StepResultExpectation("2.2.", 3, 3, nameof(Then_step_three), ExecutionStatus.Passed)
            );
        }
    }
}