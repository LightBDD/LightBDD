using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Results;
using LightBDD.Core.UnitTests.Helpers;
using LightBDD.ScenarioHelpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using IgnoreException = LightBDD.Core.Execution.IgnoreException;

namespace LightBDD.Core.UnitTests.Execution
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
    public class ExecutionPipeline_decorators_tests
    {
        private static readonly AsyncLocal<ConcurrentQueue<string>> CapturedMessages = new();

        public ExecutionPipeline_decorators_tests()
        {
            CapturedMessages.Value = new();
        }

        class MyFeature
        {
            [TestScenario]
            [MyCapturingDecorator("local2", Order = 100)]
            [MyCapturingDecorator("local1", Order = 0)]
            public Task Decorated_scenario() => TestScenarioBuilder.Current.TestScenario(Some_step1, Some_step2);

            [TestScenario]
            public Task Passing_scenario() => TestScenarioBuilder.Current.TestScenario(Some_step1);

            [TestScenario]
            [MyThrowingDecorator(ExecutionStatus.Failed)]
            public Task Throwing_scenario() => TestScenarioBuilder.Current.TestScenario(Some_step1);

            [TestScenario]
            [MyThrowingDecorator(ExecutionStatus.Ignored)]
            public Task Ignored_scenario() => TestScenarioBuilder.Current.TestScenario(Some_step1);

            [TestScenario]
            [MyThrowingDecorator(ExecutionStatus.Bypassed)]
            public Task Bypassed_scenario() => TestScenarioBuilder.Current.TestScenario(Some_step1);

            [TestScenario]
            public Task Scenario_with_failed_step() => TestScenarioBuilder.Current.TestScenario(My_failed_step);

            [TestScenario]
            public Task Scenario_with_ignored_step() => TestScenarioBuilder.Current.TestScenario(My_ignored_step);

            [TestScenario]
            public Task Scenario_with_bypassed_step() => TestScenarioBuilder.Current.TestScenario(My_bypassed_step);

            [TestScenario]
            [MyRetryDecorator]
            public async Task Retried_scenario()
            {
                int attempt = 0;

                void Some_Step()
                {
                    if (attempt++ == 0)
                        throw new InvalidOperationException("error");
                }

                void Other_Step()
                {
                }

                await TestScenarioBuilder.Current.TestScenario(Some_Step, Other_Step);
            }


            [MyThrowingDecorator(ExecutionStatus.Failed)]
            private void My_failed_step() { }

            [MyThrowingDecorator(ExecutionStatus.Ignored)]
            private void My_ignored_step() { }

            [MyThrowingDecorator(ExecutionStatus.Bypassed)]
            private void My_bypassed_step() { }

            [MyCapturingDecorator("s1-ext1", Order = 0)]
            [MyCapturingDecorator("s1-ext2", Order = 1)]
            internal void Some_step1()
            {
            }

            [MyCapturingDecorator("s2-ext1", Order = 0)]
            [MyCapturingDecorator("s2-ext2", Order = 1)]
            internal void Some_step2()
            {
            }
        }

        [Test]
        public async Task It_should_call_global_and_local_decorators_in_configured_order()
        {
            void OnConfigure(LightBddConfiguration cfg)
            {
                cfg.ExecutionExtensionsConfiguration().EnableScenarioDecorator(() => new MyCapturingDecorator("scenario-global"));
                cfg.ExecutionExtensionsConfiguration().EnableStepDecorator(() => new MyCapturingDecorator("step-global"));
            }

            await TestableCoreExecutionPipeline.Create(OnConfigure)
                .ExecuteScenario<MyFeature>(f => f.Decorated_scenario());


            Assert.That(CapturedMessages.Value.ToArray(), Is.EqualTo(new[]
            {
                $"scenario-global: {nameof(MyFeature.Decorated_scenario)}",
                $"local1: {nameof(MyFeature.Decorated_scenario)}",
                $"local2: {nameof(MyFeature.Decorated_scenario)}",
                $"step-global: {nameof(MyFeature.Some_step1)}",
                $"s1-ext1: {nameof(MyFeature.Some_step1)}",
                $"s1-ext2: {nameof(MyFeature.Some_step1)}",
                $"step-global: {nameof(MyFeature.Some_step2)}",
                $"s2-ext1: {nameof(MyFeature.Some_step2)}",
                $"s2-ext2: {nameof(MyFeature.Some_step2)}"
            }));
        }

        [Test]
        public async Task It_should_FAIL_scenario_based_on_scenario_decorator_attribute()
        {
            var scenario = await TestableCoreExecutionPipeline.Default
                .ExecuteScenario<MyFeature>(f => f.Throwing_scenario());

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario Failed: System.InvalidOperationException: failure"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            //TODO: this changed in relation to LightBDD 3.x - review
            Assert.That(scenario.GetSteps(), Is.Empty);
            //Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        [Test]

        public async Task It_should_IGNORE_scenario_based_on_scenario_decorator_attribute()
        {
            var scenario = await TestableCoreExecutionPipeline.Default
                .ExecuteScenario<MyFeature>(f => f.Ignored_scenario());

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Ignored));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario Ignored: ignore"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            //TODO: this changed in relation to LightBDD 3.x - review
            Assert.That(scenario.GetSteps(), Is.Empty);
            //Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        [Test]
        public async Task It_should_BYPASSED_scenario_based_on_scenario_decorator_attribute()
        {
            var scenario = await TestableCoreExecutionPipeline.Default
                .ExecuteScenario<MyFeature>(f => f.Bypassed_scenario());

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario Bypassed: bypassed"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            //TODO: this changed in relation to LightBDD 3.x - review
            Assert.That(scenario.GetSteps(),Is.Empty);
            //Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        [Test]
        public async Task It_should_FAIL_scenario_based_on_step_decorator_attribute()
        {
            var scenario = await TestableCoreExecutionPipeline.Default
                .ExecuteScenario<MyFeature>(f => f.Scenario_with_failed_step());

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1 Failed: System.InvalidOperationException: failure"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Failed));
        }

        [Test]
        public async Task It_should_IGNORE_scenario_based_on_step_decorator_attribute()
        {
            var scenario = await TestableCoreExecutionPipeline.Default
                .ExecuteScenario<MyFeature>(f => f.Scenario_with_ignored_step());

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Ignored));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1 Ignored: ignore"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Test]
        public async Task It_should_BYPASSED_scenario_based_on_step_decorator_attribute()
        {
            var scenario = await TestableCoreExecutionPipeline.Default
                .ExecuteScenario<MyFeature>(f => f.Scenario_with_bypassed_step());

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1 Bypassed: bypassed"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Bypassed));
        }

        [Test]
        //TODO: reimplement
        [Ignore("Retry mechanism to be rewritten")]
        public async Task It_should_clear_step_exceptions_on_retry()
        {
            var scenario = await TestableCoreExecutionPipeline.Default
                .ExecuteScenario<MyFeature>(f => f.Retried_scenario());

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Passed));
            Assert.That(scenario.StatusDetails, Is.Null);
            Assert.That(scenario.GetSteps().Select(s => s.Status), Is.All.EqualTo(ExecutionStatus.Passed));
            Assert.That(scenario.GetSteps().Select(s => s.ExecutionException), Is.All.Null);
        }

        [Test]
        public async Task ScenarioDecorator_should_have_access_to_fixture_object()
        {
            object capturedFixture = null;
            var fixture = new MyFeature();

            void OnConfigure(LightBddConfiguration cfg)
            {
                cfg.ExecutionExtensionsConfiguration().RegisterFixtureFactory(new FakeFixtureFactory(fixture));
                cfg.ExecutionExtensionsConfiguration().EnableScenarioDecorator(() => new DelegatingDecorator(scenario => capturedFixture = scenario.Fixture));
            }

            await TestableCoreExecutionPipeline.Create(OnConfigure)
                .ExecuteScenario<MyFeature>(f => f.Passing_scenario());

            Assert.AreSame(fixture, capturedFixture);
        }


        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private class MyCapturingDecorator : Attribute, IScenarioDecoratorAttribute, IStepDecoratorAttribute
        {
            private readonly string _prefix;

            public MyCapturingDecorator(string prefix)
            {
                _prefix = prefix;
            }

            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                CapturedMessages.Value.Enqueue(_prefix + ": " + scenario.Info.Name.ToString());
                return scenarioInvocation.Invoke();
            }

            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                CapturedMessages.Value.Enqueue(_prefix + ": " + step.Info.Name.ToString());
                return stepInvocation.Invoke();
            }

            public int Order { get; set; }
        }

        [AttributeUsage(AttributeTargets.Method)]
        private class MyThrowingDecorator : Attribute, IScenarioDecoratorAttribute, IStepDecoratorAttribute
        {
            private readonly ExecutionStatus _expected;
            private readonly bool _async;

            public MyThrowingDecorator(ExecutionStatus expected, bool async = false)
            {
                _expected = expected;
                _async = async;
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                return _async ? ProcessStatusAsync() : ProcessStatus();
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                return _async ? ProcessStatusAsync() : ProcessStatus();
            }

            private async Task ProcessStatusAsync()
            {
                await Task.Yield();
                await ProcessStatus();
            }

            private Task ProcessStatus()
            {
                switch (_expected)
                {
                    case ExecutionStatus.Passed:
                        return Task.CompletedTask;
                    case ExecutionStatus.Failed:
                        throw new InvalidOperationException("failure");
                    case ExecutionStatus.Bypassed:
                        throw new BypassException("bypassed");
                    case ExecutionStatus.Ignored:
                        throw new IgnoreException("ignore");
                    default:
                        throw new NotImplementedException();
                }
            }

            public int Order { get; set; }
        }

        private class MyRetryDecorator : Attribute, IScenarioDecoratorAttribute
        {
            public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                try
                {
                    await scenarioInvocation();
                    return;
                }
                catch
                {
                    await scenarioInvocation();
                }
            }

            public int Order { get; }
        }

        private class DelegatingDecorator : IScenarioDecorator
        {
            private readonly Action<IScenario> _onCall;

            public DelegatingDecorator(Action<IScenario> onCall)
            {
                _onCall = onCall;
            }

            public Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
            {
                _onCall(scenario);
                return scenarioInvocation();
            }
        }
    }
}
