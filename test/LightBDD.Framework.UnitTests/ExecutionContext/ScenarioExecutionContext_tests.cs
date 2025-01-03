﻿using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Framework.UnitTests.ExecutionContext
{
    [TestFixture]
    public class ScenarioExecutionContext_tests
    {
        private class ExplicitContext
        {
            private readonly Guid _expectedGuid = Guid.NewGuid();

            public Task Given_implicit_context_initialized()
            {
                ScenarioExecutionContext.Current.Get<ImplicitContext>().Value = _expectedGuid;
                return Task.CompletedTask;
            }

            public Task When_time_elapsed_allowing_other_scenario_to_execute_concurrently()
            {
                return Task.Delay(500);
            }

            public Task Then_implicit_context_should_be_preserved()
            {
                AssertImplicitContext("Step Assert");
                return Task.CompletedTask;
            }

            private void AssertImplicitContext(string message)
            {
                Assert.That(
                    ScenarioExecutionContext.Current.Get<ImplicitContext>().Value,
                    Is.EqualTo(_expectedGuid),
                    () => message);
            }

            public async Task Then_implicit_context_should_be_preserved_in_subtasks()
            {
                var task1 = Task.Factory.StartNew(async () =>
                {
                    AssertImplicitContext("Task.Factory.StartNew");
                    await Task.Yield();
                    AssertImplicitContext("Task.Factory.StartNew (after yield)");
                });

                var task2 = Task.Run(async () =>
                {
                    AssertImplicitContext("new Task");
                    await Task.Yield();
                    AssertImplicitContext("new Task (after yield)");
                });

                await Task.WhenAll(task1, task2);
                AssertImplicitContext("after subtasks");
            }
        }

        private class ImplicitContext : IContextProperty
        {
            public Guid Value { get; set; }
        }

        [Test]
        public async Task ScenarioExecutionContext_should_be_shared_for_all_tasks_executed_in_scenario()
        {
            var runner = CreateRunner();
            await RunScenarioAsync(runner);
        }

        [Test]
        public async Task ScenarioExecutionContext_should_be_shared_for_all_tasks_executed_in_scenario_but_unique_per_scenario()
        {
            var runner = CreateRunner();
            await Task.WhenAll(RunScenarioAsync(runner), RunScenarioAsync(runner), RunScenarioAsync(runner), RunScenarioAsync(runner), RunScenarioAsync(runner));
        }

        [Test]
        public void ScenarioExecutionContext_should_throw_if_executed_outside_of_scenario()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => ScenarioExecutionContext.Current.Get<ImplicitContext>());

            Assert.That(exception.Message, Is.EqualTo("Current task is not executing any scenarios. Ensure that operation accessing ScenarioExecutionContext is called from task running scenario."));
        }

        [Test]
        public void ValidateStepScope_should_throw_if_not_run_from_step()
        {
            var runner = CreateRunner(b => b.WithConfiguration(c => c.ExecutionExtensionsConfiguration().EnableScenarioDecorator<OutOfStepValidator>()));
            Assert.Throws<InvalidOperationException>(() => runner.RunScenario(Dummy_step))
                !.Message.ShouldBe("Current task is not executing any scenario steps. Ensure that feature is used within task running scenario step.");
        }

        [Test]
        public void ValidateScenarioScope_should_throw_if_not_run_from_scenario()
        {
            Assert.Throws<InvalidOperationException>(() => ScenarioExecutionContext.ValidateScenarioScope())
                !.Message.ShouldBe($"Current task is not executing any scenarios. Ensure that operation accessing {nameof(ScenarioExecutionContext)} is called from task running scenario.");
        }

        [Test]
        public void ValidateScenarioScope_should_pass_when_executed_in_step()
        {
            Assert.DoesNotThrow(() => CreateRunner().RunScenario(Scenario_scope_validator_step));
        }

        [Test]
        public void ValidateStepScope_should_pass_when_executed_in_step()
        {
            Assert.DoesNotThrow(() => CreateRunner().RunScenario(Step_scope_validator_step));
        }

        public class OutOfStepValidator : IScenarioDecorator
        {
            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                ScenarioExecutionContext.ValidateStepScope();
                return scenarioInvocation.Invoke();
            }
        }

        private void Dummy_step() { }
        private void Scenario_scope_validator_step() => ScenarioExecutionContext.ValidateScenarioScope();
        private void Step_scope_validator_step() => ScenarioExecutionContext.ValidateStepScope();

        private static Task RunScenarioAsync(IBddRunner runner)
        {
            return runner.WithContext(new ExplicitContext()).RunScenarioAsync(
                ctx => ctx.Given_implicit_context_initialized(),
                ctx => ctx.When_time_elapsed_allowing_other_scenario_to_execute_concurrently(),
                ctx => ctx.Then_implicit_context_should_be_preserved(),
                ctx => ctx.Then_implicit_context_should_be_preserved_in_subtasks());
        }

        private IBddRunner CreateRunner(Action<TestableIntegrationContextBuilder> onConfigure = null)
        {
            var builder = TestableIntegrationContextBuilder.Default();
            onConfigure?.Invoke(builder);
            return new TestableFeatureRunnerRepository(builder)
                .GetRunnerFor(GetType())
                .GetBddRunner(this);
        }
    }
}
