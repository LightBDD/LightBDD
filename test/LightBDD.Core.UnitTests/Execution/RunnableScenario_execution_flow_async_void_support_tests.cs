using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class RunnableScenario_execution_flow_async_void_support_tests
{
    [Test]
    public async Task Successful_run_should_result_with_passed_scenario()
    {
        var result = await TestableScenarioFactory.Default.CreateScenario(_ => Task.CompletedTask)
            .RunAsync();

        result.Status.ShouldBe(ExecutionStatus.Passed);
        result.StatusDetails.ShouldBeNull();
        result.ExecutionException.ShouldBeNull();
        result.ExecutionTime.ShouldNotBe(ExecutionTime.None);
        result.GetSteps().ShouldBeEmpty();
    }

    [Test]
    public async Task Run_should_await_for_async_void_method_before_return()
    {
        var finished = false;

        async void AsyncVoid()
        {
            await Task.Delay(200);
            finished = true;
        }

        Task EntryMethod(ICoreScenarioBuilderV2 _)
        {
            AsyncVoid();
            return Task.CompletedTask;
        }

        var result = await TestableScenarioFactory.Default.CreateScenario(EntryMethod).RunAsync();

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

        Task EntryMethod(ICoreScenarioBuilderV2 _)
        {
            DelayAndThrow();
            return Task.CompletedTask;
        }

        var result = await TestableScenarioFactory.Default.CreateScenario(EntryMethod).RunAsync();
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

        async Task EntryMethod(ICoreScenarioBuilderV2 _)
        {
            await Task.Delay(100);
            DelayAndThrow();
            DelayAndThrow();
            throw new InvalidOperationException("test2");
        };

        var result = await TestableScenarioFactory.Default.CreateScenario(EntryMethod).RunAsync();
        result.Status.ShouldBe(ExecutionStatus.Failed);
        var ex = result.ExecutionException.ShouldBeOfType<AggregateException>();

        Assert.That(ex.InnerExceptions.Select(x => x.Message), Is.EquivalentTo(new[] { "test2", "test", "test" }));
    }
}