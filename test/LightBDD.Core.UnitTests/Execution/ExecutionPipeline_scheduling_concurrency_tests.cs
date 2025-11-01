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
using LightBDD.Framework;
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
            public Task BlockingScenario(SemaphoreSlim inSem, SemaphoreSlim outSem)
            {
                inSem.Release();
                Thread.Sleep(50);
                outSem.Wait(TimeSpan.FromSeconds(5));
                return Task.CompletedTask;
            }

            [TestScenario]
            public async Task NonBlockingScenario(int delayInMs)
            {
                await Task.Delay(delayInMs);
            }

            [TestScenario]
            [RunOnDedicatedThread]
            public async Task NonBlockingScenarioWithDedicatedThread(int delayInMs)
            {
                await Task.Delay(delayInMs);
            }

            [TestScenario]
            [RunExclusively]
            public async Task ScenarioWithExclusiveRunConstraint()
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task It_should_support_parallel_scenario_execution()
        {
            var maxScenarios = 2;
            var totalScenarios = 10;
            var inSem = new SemaphoreSlim(0);
            var outSem = new SemaphoreSlim(0);
            var counter = new RunCounter();
            void Configure(LightBddConfiguration cfg)
            {
                cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(maxScenarios);
                cfg.Services.AddSingleton(counter);
                cfg.Services.ConfigureScenarioDecorators().Add<CountingDecorator>();
            }

            var scenarios = CreateBlockingScenarios(totalScenarios, inSem, outSem).ToArray();
            var executeTask = TestableCoreExecutionPipeline.Create(Configure).Execute(scenarios);

            for (int i = 0; i < maxScenarios; ++i)
                await inSem.WaitAsync(TimeSpan.FromSeconds(5));
            outSem.Release(totalScenarios);

            var result = await executeTask;
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

        [Test]
        public async Task It_should_support_concurrent_execution_across_schedulers()
        {
            var delayInMs = 250;
            var maxScenarios = 50;
            var countPerScheduler = 45;
            var counter = new RunCounter();
            void Configure(LightBddConfiguration cfg)
            {
                cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(maxScenarios);
                cfg.Services.AddSingleton(counter);
                cfg.Services.ConfigureScenarioDecorators().Add<CountingDecorator>();
            }

            var scenarios = CreateNonBlockingScenariosWithDedicatedThread<object>(countPerScheduler, delayInMs)
                .Concat(CreateNonBlockingScenarios<object>(countPerScheduler, delayInMs))
                .ToArray();

            var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(scenarios);
            result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

            counter.Total.ShouldBe(scenarios.Length);
            counter.Max.ShouldBe(maxScenarios);
        }

        [Test]
        public async Task It_should_support_exclusive_run_constraint()
        {
            var maxScenarios = 50;
            var counter = new RunCounter();
            void Configure(LightBddConfiguration cfg)
            {
                cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(maxScenarios);
                cfg.Services.AddSingleton(counter);
                cfg.Services.ConfigureScenarioDecorators().Add<CountingDecorator>();
            }

            var methodInfo = typeof(MyFeature<MyFeature<object>>).GetMethod(nameof(MyFeature<object>.ScenarioWithExclusiveRunConstraint))!;
            var scenarios = Enumerable.Range(0, maxScenarios)
                .Select(_ => ScenarioCase.CreateParameterless(typeof(MyFeature<MyFeature<object>>).GetTypeInfo(), methodInfo))
                .ToArray();

            var result = await TestableCoreExecutionPipeline.Create(Configure).Execute(scenarios);
            result.OverallStatus.ShouldBe(ExecutionStatus.Passed);

            counter.Total.ShouldBe(scenarios.Length);
            counter.Max.ShouldBe(1);
        }

        private static IEnumerable<ScenarioCase> CreateNonBlockingScenariosWithDedicatedThread<T>(int totalScenarios, int delayInMs)
        {
            var nonBlockingScenario = typeof(MyFeature<T>).GetMethod(nameof(MyFeature<T>.NonBlockingScenarioWithDedicatedThread))!;
            return Enumerable.Range(0, totalScenarios)
                .Select(_ => ScenarioCase.CreateParameterized(typeof(MyFeature<T>).GetTypeInfo(), nonBlockingScenario, new object[] { delayInMs }));
        }


        private static IEnumerable<ScenarioCase> CreateNonBlockingScenarios<T>(int totalScenarios, int delayInMs)
        {
            var nonBlockingScenario = typeof(MyFeature<T>).GetMethod(nameof(MyFeature<T>.NonBlockingScenario))!;
            return Enumerable.Range(0, totalScenarios)
                .Select(_ => ScenarioCase.CreateParameterized(typeof(MyFeature<T>).GetTypeInfo(), nonBlockingScenario, new object[] { delayInMs }));
        }

        private static IEnumerable<ScenarioCase> CreateBlockingScenarios(int totalScenarios, SemaphoreSlim inSem, SemaphoreSlim outSem)
        {
            var blockingScenario = typeof(MyFeature<object>).GetMethod(nameof(MyFeature<object>.BlockingScenario))!;
            return Enumerable.Range(0, totalScenarios)
                .Select(_ => ScenarioCase.CreateParameterized(typeof(MyFeature<object>).GetTypeInfo(), blockingScenario, new object[] { inSem, outSem }));
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
