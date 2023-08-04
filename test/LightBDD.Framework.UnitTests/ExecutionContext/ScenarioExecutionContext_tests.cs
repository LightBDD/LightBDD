using System;
using System.Threading.Tasks;
using LightBDD.Core.ExecutionContext;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

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
            var runner = TestableBddRunner.Default;
            await RunScenarioAsync(runner);
        }

        [Test]
        public async Task ScenarioExecutionContext_should_be_shared_for_all_tasks_executed_in_scenario_but_unique_per_scenario()
        {
            var runner = TestableBddRunner.Default;
            await Task.WhenAll(RunScenarioAsync(runner), RunScenarioAsync(runner), RunScenarioAsync(runner), RunScenarioAsync(runner), RunScenarioAsync(runner));
        }

        [Test]
        public void ScenarioExecutionContext_should_throw_if_executed_outside_of_scenario()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => ScenarioExecutionContext.Current.Get<ImplicitContext>());

            Assert.That(exception.Message, Is.EqualTo("Current task is not executing any scenarios. Ensure that operation accessing ScenarioExecutionContext is called from task running scenario."));
        }

        private static Task RunScenarioAsync(TestableBddRunner runner)
        {
            return runner.RunScenario(r => r.WithContext(new ExplicitContext()).RunScenarioAsync(
                ctx => ctx.Given_implicit_context_initialized(),
                ctx => ctx.When_time_elapsed_allowing_other_scenario_to_execute_concurrently(),
                ctx => ctx.Then_implicit_context_should_be_preserved(),
                ctx => ctx.Then_implicit_context_should_be_preserved_in_subtasks()));
        }
    }


}
