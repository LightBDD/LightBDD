using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Extensibility;
using LightBDD.ScenarioHelpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class RunnableScenario_multiassert_step_execution_tests
    {
        private void Passing_step() { }
        private void Failing_step() { throw new InvalidOperationException("failing"); }
        private void Ignoring_step() { StepExecution.Current.Ignore("ignoring"); }

        [MultiAssert]
        private TestCompositeStep Multiassert_ignoring_steps()
        {
            return TestCompositeStep.Create(Ignoring_step, Ignoring_step, Passing_step);
        }
        [MultiAssert]
        private TestCompositeStep Multiassert_failing_composite()
        {
            return TestCompositeStep.Create(Failing_step, Failing_step, Passing_step);
        }

        private TestCompositeStep Passing_composite()
        {
            return TestCompositeStep.Create(Passing_step, Passing_step);
        }

        [Test]
        public async Task Runner_should_throw_only_first_ignore_exception_to_be_properly_handled_by_scenario_runner()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Multiassert_ignoring_steps));
            scenario.Status.ShouldBe(ExecutionStatus.Ignored);
            scenario.ExecutionException.ShouldBeOfType<Core.Execution.IgnoreException>();
        }

        [Test]
        public async Task Runner_should_propagate_exceptions_from_multiassert_scenario_with_AggregateException()
        {
            var scenario = await TestableScenarioFactory.Default
                .CreateBuilder()
                .WithScenarioDecorators(new[] { new MultiAssertAttribute() })
                .WithScenarioEntryMethod((_, r) => r.Test().TestGroupScenario(
                    Multiassert_failing_composite,
                    Multiassert_failing_composite))
                .Build()
                .RunAsync();

            var ex = scenario.ExecutionException.ShouldBeOfType<AggregateException>();

            Assert.That(ex.InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(AggregateException), typeof(AggregateException) }));
            var innerAggregates = ex.InnerExceptions.Cast<AggregateException>().ToArray();
            Assert.That(innerAggregates[0].InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(InvalidOperationException), typeof(InvalidOperationException) }));
            Assert.That(innerAggregates[1].InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(InvalidOperationException), typeof(InvalidOperationException) }));
        }

        [Test]
        public async Task Runner_should_throw_original_exception_if_there_is_no_need_to_aggregate_multiple_ones()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(Multiassert_failing_composite));

            var ex = scenario.ExecutionException.ShouldBeOfType<AggregateException>();
            Assert.That(ex.InnerExceptions.Select(x => x.GetType()).ToArray(), Is.EqualTo(new[] { typeof(InvalidOperationException), typeof(InvalidOperationException) }));
        }

        [Test]
        public async Task Runner_should_aggregate_only_failure_exceptions()
        {
            var scenario = await TestableScenarioFactory.Default
                .CreateBuilder()
                .WithScenarioDecorators(new[] { new MultiAssertAttribute() })
                .WithScenarioEntryMethod((_, r) => r.Test().TestGroupScenario(
                    Multiassert_failing_composite,
                    Multiassert_ignoring_steps,
                    Multiassert_failing_composite))
                .Build()
                .RunAsync();

            var ex = scenario.ExecutionException.ShouldBeOfType<AggregateException>();

            Assert.That(
                ex.Flatten().InnerExceptions.Select(x => x.GetType()).ToArray(),
                Is.EqualTo(new[]
                {
                    typeof(InvalidOperationException), typeof(InvalidOperationException),
                    typeof(InvalidOperationException), typeof(InvalidOperationException)
                }));
        }

        [Test]
        public async Task Runner_should_properly_capture_status_from_multiassert_steps()
        {
            var scenario = await TestableScenarioFactory.Default
                .CreateBuilder()
                .WithScenarioDecorators(new[] { new MultiAssertAttribute() })
                .WithScenarioEntryMethod((_, r) => r.Test().TestGroupScenario(
                    Multiassert_ignoring_steps,
                    Multiassert_failing_composite,
                    Passing_composite))
                .Build()
                .RunAsync();

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));

            var steps = scenario.GetSteps().ToArray();
            StepResultExpectation.AssertEqual(steps,
                new StepResultExpectation(1, 3, nameof(Multiassert_ignoring_steps), ExecutionStatus.Ignored, $"Step 1.1 Ignored: ignoring{Environment.NewLine}Step 1.2 Ignored: ignoring"),
                new StepResultExpectation(2, 3, nameof(Multiassert_failing_composite), ExecutionStatus.Failed, $"Step 2.1 Failed: System.InvalidOperationException: failing{Environment.NewLine}Step 2.2 Failed: System.InvalidOperationException: failing"),
                new StepResultExpectation(3, 3, nameof(Passing_composite), ExecutionStatus.Passed)
                );

            StepResultExpectation.AssertEqual(steps[0].GetSubSteps(),
                new StepResultExpectation("1.", 1, 3, nameof(Ignoring_step), ExecutionStatus.Ignored, "Step 1.1 Ignored: ignoring"),
                new StepResultExpectation("1.", 2, 3, nameof(Ignoring_step), ExecutionStatus.Ignored, "Step 1.2 Ignored: ignoring"),
                new StepResultExpectation("1.", 3, 3, nameof(Passing_step), ExecutionStatus.Passed)
            );

            StepResultExpectation.AssertEqual(steps[1].GetSubSteps(),
                new StepResultExpectation("2.", 1, 3, nameof(Failing_step), ExecutionStatus.Failed, "Step 2.1 Failed: System.InvalidOperationException: failing"),
                new StepResultExpectation("2.", 2, 3, nameof(Failing_step), ExecutionStatus.Failed, "Step 2.2 Failed: System.InvalidOperationException: failing"),
                new StepResultExpectation("2.", 3, 3, nameof(Passing_step), ExecutionStatus.Passed)
            );

            StepResultExpectation.AssertEqual(steps[2].GetSubSteps(),
                new StepResultExpectation("3.", 1, 2, nameof(Passing_step), ExecutionStatus.Passed),
                new StepResultExpectation("3.", 2, 2, nameof(Passing_step), ExecutionStatus.Passed)
            );
        }
    }
}