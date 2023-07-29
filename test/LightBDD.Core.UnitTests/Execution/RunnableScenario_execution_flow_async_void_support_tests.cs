﻿using System;
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
    public async Task Run_should_await_for_async_void_method_before_return()
    {
        var finished = false;

        async void AsyncVoid()
        {
            await Task.Delay(200);
            finished = true;
        }

        Task EntryMethod(ICoreScenarioStepsRunner _)
        {
            AsyncVoid();
            return Task.CompletedTask;
        }

        var result = await TestableScenarioFactory.Default.RunScenario(EntryMethod);

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

        Task EntryMethod(ICoreScenarioStepsRunner _)
        {
            DelayAndThrow();
            return Task.CompletedTask;
        }

        var result = await TestableScenarioFactory.Default.RunScenario(EntryMethod);
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

        async Task EntryMethod(ICoreScenarioStepsRunner _)
        {
            await Task.Delay(100);
            DelayAndThrow();
            DelayAndThrow();
            throw new InvalidOperationException("test2");
        }

        var result = await TestableScenarioFactory.Default.RunScenario(EntryMethod);
        result.Status.ShouldBe(ExecutionStatus.Failed);
        var ex = result.ExecutionException.ShouldBeOfType<AggregateException>();

        Assert.That(ex.InnerExceptions.Select(x => x.Message), Is.EquivalentTo(new[] { "test2", "test", "test" }));
    }
}