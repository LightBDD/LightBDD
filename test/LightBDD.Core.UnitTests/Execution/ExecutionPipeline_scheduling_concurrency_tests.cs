using System;
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
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    public class ExecutionPipeline_scheduling_concurrency_tests
    {
        class MyFeature<T>
        {
            [TestScenario]
            public Task BlockingScenario(int delayInMs)
            {
                Thread.Sleep(delayInMs);
                return Task.CompletedTask;
            }

            [TestScenario]
            public async Task NonBlockingScenario(int delayInMs)
            {
                await Task.Delay(delayInMs);
            }
        }

        [Test]
        public async Task It_should_support_parallel_scenario_execution()
        {
            var delayInMs = 250;
            var maxScenarios = 2;
            var totalScenarios = 4;
            var counter = new RunCounter();
            void Configure(LightBddConfiguration cfg)
            {
                cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(maxScenarios);
                cfg.Services.AddSingleton(counter);
                cfg.Services.ConfigureScenarioDecorators().Add<CountingDecorator>();
            }

            var scenarios = CreateBlockingScenarios<object>(totalScenarios, delayInMs).ToArray();

            var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(scenarios);
            result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

            counter.Total.ShouldBe(totalScenarios);
            counter.Max.ShouldBe(maxScenarios);
        }

        [Test]
        public async Task It_should_support_asynchronous_scenario_execution()
        {
            var delayInMs = 250;
            var maxScenarios = 50;
            var totalScenarios = 100;
            var counter = new RunCounter();
            void Configure(LightBddConfiguration cfg)
            {
                cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(maxScenarios);
                cfg.Services.AddSingleton(counter);
                cfg.Services.ConfigureScenarioDecorators().Add<CountingDecorator>();
            }

            var scenarios = CreateNonBlockingScenarios<object>(totalScenarios, delayInMs).ToArray();

            var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(scenarios);
            result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

            counter.Total.ShouldBe(totalScenarios);
            counter.Max.ShouldBe(maxScenarios);
        }

        [Test]
        public async Task It_should_support_concurrent_execution_across_features()
        {
            var delayInMs = 250;
            var maxScenarios = 50;
            var countPerFeature = 20;
            var counter = new RunCounter();
            void Configure(LightBddConfiguration cfg)
            {
                cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(maxScenarios);
                cfg.Services.AddSingleton(counter);
                cfg.Services.ConfigureScenarioDecorators().Add<CountingDecorator>();
            }

            var scenarios = CreateNonBlockingScenarios<object>(countPerFeature, delayInMs)
                .Concat(CreateNonBlockingScenarios<int>(countPerFeature, delayInMs))
                .Concat(CreateNonBlockingScenarios<DateTime>(countPerFeature, delayInMs))
                .Concat(CreateNonBlockingScenarios<string>(countPerFeature, delayInMs))
                .ToArray();

            var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(scenarios);
            result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

            counter.Total.ShouldBe(scenarios.Length);
            counter.Max.ShouldBe(maxScenarios);
        }

        private static IEnumerable<ScenarioCase> CreateNonBlockingScenarios<T>(int totalScenarios, int delayInMs)
        {
            var nonBlockingScenario = typeof(MyFeature<T>).GetMethod(nameof(MyFeature<T>.NonBlockingScenario))!;
            return Enumerable.Range(0, totalScenarios)
                .Select(_ => ScenarioCase.CreateParameterized(typeof(MyFeature<T>).GetTypeInfo(), nonBlockingScenario, new object[] { delayInMs }));
        }

        private static IEnumerable<ScenarioCase> CreateBlockingScenarios<T>(int totalScenarios, int delayInMs)
        {
            var blockingScenario = typeof(MyFeature<T>).GetMethod(nameof(MyFeature<T>.BlockingScenario))!;
            return Enumerable.Range(0, totalScenarios)
                .Select(_ => ScenarioCase.CreateParameterized(typeof(MyFeature<T>).GetTypeInfo(), blockingScenario, new object[] { delayInMs }));
        }

        class CountingDecorator : IScenarioDecorator
        {
            private readonly RunCounter _counter;

            public CountingDecorator(RunCounter counter)
            {
                _counter = counter;
            }

            public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                _counter.Increment();
                try
                {
                    await scenarioInvocation();
                }
                finally
                {
                    _counter.Decrement();
                }
            }
        }

        class RunCounter
        {
            private int _max;
            private int _current;
            public int Max => _max;
            public int Total { get; private set; }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Increment()
            {
                _max = Math.Max(_max, ++_current);
                ++Total;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            public void Decrement() => --_current;
        }
    }
}
