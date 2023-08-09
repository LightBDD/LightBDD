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
    //TODO: review ability to run scenario twice on the same scenario builder and either disallow it or ensure the context is created twice
    public class RunnableScenario_step_execution_with_context_tests
    {
        [Test]
        public async Task Runner_should_instantiate_context_just_before_run_so_its_failure_would_be_included_in_results()
        {
            var scenario = await TestableScenarioFactory.Default.RunScenario(r => r.Test()
                .WithContext(() => throw new InvalidOperationException("abc"), false)
                .TestScenario(Given_step_one));

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario Failed: System.InvalidOperationException: Step group context initialization failed: abc"));
        }

        [Test]
        public async Task Runner_should_instantiate_context_and_use_it_in_step_and_parameter_execution()
        {
            var someParameterInfo = ParameterInfoHelper.GetMethodInfo<int>(Given_step_two).GetParameters()[0];

            var list = new List<object>();

            await TestableScenarioFactory.Default.RunScenario(r => r.Test()
                .WithContext(() => new object(), false)
                .TestScenario(
                    new StepDescriptor("test1", (ctx, _) =>
                    {
                        list.Add(ctx);
                        return Task.FromResult(DefaultStepResultDescriptor.Instance);
                    }, ParameterDescriptor.FromInvocation(someParameterInfo, ctx =>
                    {
                        list.Add(ctx);
                        return ctx;
                    })),
                    new StepDescriptor("test1", (ctx, _) =>
                    {
                        list.Add(ctx);
                        return Task.FromResult(DefaultStepResultDescriptor.Instance);
                    }, ParameterDescriptor.FromInvocation(someParameterInfo, ctx =>
                    {
                        list.Add(ctx);
                        return ctx;
                    }))));

            Assert.That(list.Count, Is.EqualTo(4));
            Assert.That(list.Distinct().Count, Is.EqualTo(1), "Only one instance of object should be created");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task Runner_should_dispose_context_depending_on_takeOwnership_flag(bool shouldDispose)
        {
            var context = Mock.Of<IDisposable>();
            await TestableScenarioFactory.Default.RunScenario(r => r.Test()
                .WithContext(() => context, shouldDispose)
                .TestScenario(Given_step_one));
            Mock.Get(context).Verify(x => x.Dispose(), Times.Exactly(shouldDispose ? 1 : 0));
        }

        [Test]
        public async Task Runner_should_propagate_context_disposal_exception()
        {
            var exception = new Exception("foo");
            var context = Mock.Of<IDisposable>();
            Mock.Get(context).Setup(x => x.Dispose()).Throws(exception);

            var scenario = await TestableScenarioFactory.Default.RunScenario(r => r.Test()
                .WithContext(() => context, true)
                .TestScenario(Given_step_one));

            var ex = scenario.ExecutionException.ShouldBeOfType<InvalidOperationException>();
            Assert.That(ex.Message, Is.EqualTo($"DI Scope Dispose() failed: Failed to dispose transient dependency '{context.GetType().Name}': foo"));
            Assert.That(ex.InnerException?.InnerException, Is.SameAs(exception));
            Assert.That(ex.StackTrace, Is.Not.Null);
        }

        [Test]
        public async Task Runner_should_propagate_context_disposal_exception_together_with_failing_scenario()
        {
            var exception = new InvalidOperationException("foo");
            var context = Mock.Of<IDisposable>();
            Mock.Get(context).Setup(x => x.Dispose()).Throws(exception);

            var scenario = await TestableScenarioFactory.Default.RunScenario(r => r.Test()
                .WithContext(() => context, true)
                .TestScenario(Step_throwing_exception));

            var ex = scenario.ExecutionException.ShouldBeOfType<AggregateException>();
            Assert.That(ex.InnerExceptions.Select(x => $"{x.GetType().Name}|{x.Message}").ToArray(),
                Is.EquivalentTo(new[]
                {
                    $"{nameof(Exception)}|bar",
                    $"{nameof(InvalidOperationException)}|DI Scope Dispose() failed: Failed to dispose transient dependency '{context.GetType().Name}': foo"
                }));
        }

        private void Step_throwing_exception()
        {
            throw new Exception("bar");
        }
        private void Given_step_one() { }
        private void Given_step_two(int parameter) { }
    }
}