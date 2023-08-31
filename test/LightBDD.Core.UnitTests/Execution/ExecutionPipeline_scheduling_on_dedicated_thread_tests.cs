using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Discovery;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.Framework;
using LightBDD.Framework.Reporting;
using LightBDD.ScenarioHelpers;
using NUnit.Framework;
using Shouldly;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [Parallelizable]
    public class ExecutionPipeline_scheduling_on_dedicated_thread_tests
    {
        class MyFixture
        {
            private readonly Guid _value = Guid.NewGuid();
            private readonly ThreadLocal<Guid> _threadLocal = new();
            private readonly int _threadId;

            public MyFixture()
            {
                _threadLocal.Value = _value;
                _threadId = Environment.CurrentManagedThreadId;
            }

            [TestScenario]
            [RunOnDedicatedThread]
            public Task Scenario_with_composites() => TestScenarioBuilder.Current.TestGroupScenario(Composite_step, Composite_step);

            [TestScenario]
            [RunOnDedicatedThread]
            public Task Scenario_with_tasks_continuations_executed_on_thread_pool() => TestScenarioBuilder.Current.TestScenario(Step_with_tasks_continuations_executed_on_thread_pool);

            [TestScenario]
            [RunOnDedicatedThread]
            public async Task Scenario_with_thread_capture(ConcurrentQueue<int> capture)
            {
                await Task.Yield();
                capture.Enqueue(Environment.CurrentManagedThreadId);
            }

            private TestCompositeStep Composite_step() => TestCompositeStep.Create(Enumerable.Repeat(Step_with_tasks, 100).ToArray());

            private async Task Step_with_tasks()
            {
                VerifyTaskExecutedOnSameThread();
                await Task.WhenAll(AsyncTask(), AsyncTask());
                VerifyTaskExecutedOnSameThread();
            }

            private async Task Step_with_tasks_continuations_executed_on_thread_pool()
            {
                await Task.WhenAll(AsyncTaskWithConfigureAwait(), AsyncTaskWithConfigureAwait());
                VerifyTaskExecutedOnSameThread();
            }

            private async Task AsyncTask()
            {
                await Task.Yield();
                VerifyTaskExecutedOnSameThread();
            }

            private async Task AsyncTaskWithConfigureAwait()
            {
                await Task.Delay(5).ConfigureAwait(false);
                VerifyTaskExecutedOnDifferentThread();
            }

            private void VerifyTaskExecutedOnDifferentThread()
            {
                Environment.CurrentManagedThreadId.ShouldNotBe(_threadId, "Task should be executed on different thread");
                _threadLocal.Value.ShouldNotBe(_value, "ThreadLocal value should not be available for this task");
            }

            public void VerifyTaskExecutedOnSameThread()
            {
                Environment.CurrentManagedThreadId.ShouldBe(_threadId, "Task executed on different thread");
                _threadLocal.Value.ShouldBe(_value, "ThreadLocal value did not go through to the task");
            }
        }

        class AsyncDecorator : IScenarioDecorator, IStepDecorator
        {
            public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                await Task.Delay(50);
                await scenarioInvocation();
                ((MyFixture)scenario.Fixture).VerifyTaskExecutedOnSameThread();
            }

            public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                await Task.Yield();
                await stepInvocation();
            }
        }

        [Test]
        public async Task It_should_execute_all_the_steps_in_same_thread_as_scenario_and_allow_sharing_data_via_ThreadLocalStorage()
        {
            var result = await TestableCoreExecutionPipeline
                .Create(OnConfigure)
                .ExecuteScenario<MyFixture>(f => f.Scenario_with_composites());

            result.Status.ShouldBe(ExecutionStatus.Passed, result.ExecutionException?.ToString());

            var stepResults = result.GetAllSteps().ToArray();
            stepResults.Length.ShouldBe(202);
            stepResults.ShouldAllBe(s => s.Status == ExecutionStatus.Passed);
        }

        [Test]
        public async Task It_should_allow_running_sub_tasks_on_standard_thread_pool_with_ConfigureAwait()
        {
            var result = await TestableCoreExecutionPipeline
                .Create(OnConfigure)
                .ExecuteScenario<MyFixture>(f => f.Scenario_with_tasks_continuations_executed_on_thread_pool());

            result.Status.ShouldBe(ExecutionStatus.Passed, result.ExecutionException?.ToString());
        }

        [Test]
        public async Task It_should_reuse_threads_that_finished_executing_scenarios()
        {
            var totalScenarioCount = 100;
            var maxConcurrentScenarios = 3;

            var capture = new ConcurrentQueue<int>();
            var methodInfo = typeof(MyFixture).GetMethod(nameof(MyFixture.Scenario_with_thread_capture))!;
            var scenarioCases = Enumerable.Range(0, totalScenarioCount)
                .Select(_ => ScenarioCase.CreateParameterized(typeof(MyFixture).GetTypeInfo(), methodInfo, new object[] { capture }))
                .ToArray();

            var results = await TestableCoreExecutionPipeline.Create(cfg => cfg.ForExecutionPipeline().SetMaxConcurrentScenarios(maxConcurrentScenarios))
                .Execute(scenarioCases);

            results.OverallStatus.ShouldBe(ExecutionStatus.Passed);

            // There is a potential race condition in SingleThreadScenarioRunner where the OnFinish callback putting the runner back to the pool may be executed after scenario task being reported back as completed, which then may cause scheduling another scenario on the pool with empty runner list.
            // At the point of writing this test this behavior seems to be acceptable.
            capture.Distinct().Count().ShouldBeInRange(maxConcurrentScenarios, 2 * maxConcurrentScenarios);
        }

        private void OnConfigure(LightBddConfiguration cfg)
        {
            cfg.Services.ConfigureScenarioDecorators().Add<AsyncDecorator>();
            cfg.Services.ConfigureStepDecorators().Add<AsyncDecorator>();
        }
    }
}