using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class RunnableScenario_step_execution_async_void_flow_tests
{
    [Test]
    public async Task It_should_run_provided_steps()
    {
        var finished = false;

        async void AsyncVoid()
        {
            await Task.Delay(200);
            finished = true;
        }

        Task MyScenario(ICoreScenarioStepsRunner runner) => runner.Test().TestScenario(AsyncVoid);

        var result = await TestableScenarioFactory.Default.RunScenario(MyScenario);
        finished.ShouldBe(true);
        result.Status.ShouldBe(ExecutionStatus.Passed);
    }

    [Test]
    public async Task Run_should_await_for_async_void_step_and_propagate_any_exceptions_thrown()
    {
        async void DelayAndThrow()
        {
            await Task.Delay(200);
            throw new InvalidOperationException("test");
        }

        Task MyScenario(ICoreScenarioStepsRunner runner) => runner.Test().TestScenario(DelayAndThrow);

        var result = await TestableScenarioFactory.Default.RunScenario(MyScenario);
        result.Status.ShouldBe(ExecutionStatus.Failed);
        var ex = result.ExecutionException.ShouldBeOfType<InvalidOperationException>();

        Assert.That(ex.Message, Is.EqualTo("test"));
    }

    [Test]
    public async Task Run_should_await_for_inner_async_void_methods_and_propagate_their_exceptions()
    {
        async void DelayAndThrow()
        {
            await Task.Delay(200);
            throw new InvalidOperationException("test");
        }

        async Task AsyncVoidStep()
        {
            await Task.Delay(100);
            DelayAndThrow();
            DelayAndThrow();
            throw new InvalidOperationException("test2");
        }

        Task MyScenario(ICoreScenarioStepsRunner runner) => runner.Test().TestScenario(AsyncVoidStep);

        var result = await TestableScenarioFactory.Default.RunScenario(MyScenario);
        result.Status.ShouldBe(ExecutionStatus.Failed);
        var ex = result.ExecutionException.ShouldBeOfType<AggregateException>();

        Assert.That(ex.InnerExceptions.Select(x => x.Message), Is.EquivalentTo(new[] { "test2", "test", "test" }));
    }
}