using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class RunnableScenario_hierarchical_step_execution_with_context_tests : Steps
    {
        private readonly List<(string step, object context)> _capturedSteps = new();

        [Test]
        public async Task Runner_should_execute_composite_steps_with_own_context_objects()
        {
            await TestableScenarioFactory.Default.RunScenario(r =>
                    r.Test().TestGroupScenario(
                        StepGroupWithContext1,
                        StepGroupWithContext2,
                        StepGroupWithoutContext));

            var stepGroups = _capturedSteps.GroupBy(x => x.context).ToArray();
            Assert.That(stepGroups.Length, Is.EqualTo(3), "There should be 3 different contexts");
            Assert.That(stepGroups.All(x => x.Count() == 2), Is.True, "There should be 2 steps per context");
            Assert.That(stepGroups.Any(x => x.Key == null), Is.True, "One group should have no context");

            var actualSteps = stepGroups.Select(x => x.Select(y => y.step).ToArray()).ToArray();
            var expectedSteps = new[]
            {
                new []{"step1","step2"},
                new []{"stepA","stepB"},
                new []{"stepN1","stepN2"}
            };

            Assert.That(actualSteps, Is.EqualTo(expectedSteps));
        }

        [Test]
        public async Task Runner_should_instantiate_context_just_before_run_so_its_failure_would_be_included_in_results()
        {
            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(StepGroupWithInvalidContext));

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1 Failed: System.InvalidOperationException: Step group context initialization failed: abc"));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Runner_should_dispose_context_depending_on_takeOwnership_flag(bool shouldDispose)
        {
            var context = Mock.Of<IDisposable>();

            TestCompositeStep StepGroupWithDisposable()
            {
                return new TestCompositeStep(new ExecutionContextDescriptor(() => context, shouldDispose), MakeStep("step"));
            }

            await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(StepGroupWithDisposable));
            Mock.Get(context).Verify(x => x.Dispose(), Times.Exactly(shouldDispose ? 1 : 0));
        }

        [Test]
        public async Task Runner_should_dispose_context_after_last_step()
        {
            var context = Mock.Of<IDisposable>();

            Task<IStepResultDescriptor> VerifyNotDisposed(object ctx, object[] args)
            {
                Mock.Get((IDisposable)ctx).Verify(x => x.Dispose(), Times.Never);
                return Task.FromResult(DefaultStepResultDescriptor.Instance);
            }
            TestCompositeStep StepGroupWithDisposable()
            {
                return new TestCompositeStep(
                    new ExecutionContextDescriptor(() => context, true),
                    new StepDescriptor("step1", VerifyNotDisposed),
                    new StepDescriptor("step2", VerifyNotDisposed));
            }

            await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(StepGroupWithDisposable));

            Mock.Get(context).Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public async Task Runner_should_propagate_context_disposal_exception()
        {
            var exception = new Exception("foo");
            var context = Mock.Of<IDisposable>();
            Mock.Get(context).Setup(x => x.Dispose()).Throws(exception);

            TestCompositeStep StepGroupWithDisposable()
            {
                return new TestCompositeStep(new ExecutionContextDescriptor(() => context, true), MakeStep("step"));
            }

            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(StepGroupWithDisposable));

            var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
            Assert.That(ex.Message, Is.EqualTo($"Failed to dispose dependency 'ContextWrapper': Failed to dispose context '{context.GetType().Name}': foo"));
            Assert.That(ex.InnerException?.InnerException, Is.SameAs(exception));
            Assert.That(ex.StackTrace, Is.Not.Null);
        }

        [Test]
        public async Task Runner_should_propagate_context_disposal_exception_together_with_failing_scenario()
        {
            var exception = new InvalidOperationException("foo");
            var context = Mock.Of<IDisposable>();
            Mock.Get(context).Setup(x => x.Dispose()).Throws(exception);

            TestCompositeStep StepGroupWithDisposable()
            {
                return new TestCompositeStep(
                    new ExecutionContextDescriptor(() => context, true),
                    new StepDescriptor("step", (ctx, args) => throw new Exception("bar")));
            }

            var scenario = await TestableScenarioFactory.Default
                .RunScenario(r => r.Test().TestGroupScenario(StepGroupWithDisposable));

            var ex = scenario.ExecutionException.ShouldBeOfType<AggregateException>();
            Assert.That(ex.InnerExceptions.Select(x => $"{x.GetType().Name}|{x.Message}").ToArray(),
                Is.EquivalentTo(new[]
                {
                    $"{nameof(Exception)}|bar",
                    $"{nameof(InvalidOperationException)}|Failed to dispose dependency 'ContextWrapper': Failed to dispose context '{context.GetType().Name}': foo"
                }));
        }

        private TestCompositeStep StepGroupWithInvalidContext()
        {
            return new TestCompositeStep(
                () => throw new Exception("abc"),
                MakeStep("step"));
        }

        private TestCompositeStep StepGroupWithContext1()
        {
            return new TestCompositeStep(
                () => Guid.NewGuid(),
                MakeStep("step1"),
                MakeStep("step2"));
        }

        private TestCompositeStep StepGroupWithContext2()
        {
            return new TestCompositeStep(
                () => Guid.NewGuid(),
                MakeStep("stepA"),
                MakeStep("stepB"));
        }

        private TestCompositeStep StepGroupWithoutContext()
        {
            return new TestCompositeStep(
                MakeStep("stepN1"),
                MakeStep("stepN2"));
        }

        private StepDescriptor MakeStep(string name)
        {
            return new StepDescriptor(name, (ctx, args) =>
            {
                _capturedSteps.Add((name, ctx));
                return Task.FromResult(DefaultStepResultDescriptor.Instance);
            });
        }
    }
}