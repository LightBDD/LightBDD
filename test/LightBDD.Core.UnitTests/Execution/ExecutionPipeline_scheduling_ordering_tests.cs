using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution;

[TestFixture]
public class ExecutionPipeline_scheduling_ordering_tests
{
    class MyFeature1
    {
        [TestScenario]
        [ScenarioPriority(ScenarioPriority.High)]
        public Task High1() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.High)]
        public Task High2() => Task.CompletedTask;

        [TestScenario]
        public Task Normal2() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.Normal)]
        public Task Normal1() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.Low)]
        public Task Low1() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.Low)]
        public Task Low2() => Task.CompletedTask;
    }

    class MyFeature2
    {
        [TestScenario]
        [ScenarioPriority(ScenarioPriority.High)]
        public Task High2() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.High)]
        public Task High1() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.Normal)]
        public Task Normal1() => Task.CompletedTask;

        [TestScenario]
        public Task Normal2() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.Low)]
        public Task Low2() => Task.CompletedTask;

        [TestScenario]
        [ScenarioPriority(ScenarioPriority.Low)]
        public Task Low1() => Task.CompletedTask;
    }

    [Test]
    public async Task It_should_order_scenarios_by_priority_then_fixtures_then_names()
    {
        var decorator = new CapturingDecorator();

        void Configure(LightBddConfiguration cfg)
        {
            cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(1);
            cfg.Services.ConfigureScenarioDecorators().Add(decorator);
        }

        var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(typeof(MyFeature2), typeof(MyFeature1));
        result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

        decorator.Captured.ShouldBe(new[]
        {
            "MyFeature1_High1",
            "MyFeature1_High2",
            "MyFeature2_High1",
            "MyFeature2_High2",
            "MyFeature1_Normal1",
            "MyFeature1_Normal2",
            "MyFeature2_Normal1",
            "MyFeature2_Normal2",
            "MyFeature1_Low1",
            "MyFeature1_Low2",
            "MyFeature2_Low1",
            "MyFeature2_Low2"
        });
    }

    class CapturingDecorator : IScenarioDecorator
    {
        public readonly ConcurrentQueue<string> Captured = new();

        public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            Captured.Enqueue($"{scenario.Info.Parent.Name}_{scenario.Info.Name}");
            await scenarioInvocation();
        }
    }
}