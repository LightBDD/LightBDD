using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.UnitTests.Helpers;
using LightBDD.UnitTests.Helpers.TestableIntegration;
using NUnit.Framework;

namespace LightBDD.Core.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class CoreBddRunner_execution_extension_tests
    {
        private static readonly List<string> CapturedMessages = new List<string>();

        [SetUp]
        public void SetUp() { CapturedMessages.Clear(); }

        [Test]
        [MyCapturingDecorator("local2", Order = 100)]
        [MyCapturingDecorator("local1", Order = 0)]
        public void It_should_call_global_and_local_extensions_in_order_when_scenario_is_executed()
        {
            var featureRunner = CreateRunner(cfg =>
            {
                cfg.ExecutionExtensionsConfiguration().EnableScenarioDecorator(() => new MyCapturingDecorator("scenario-global"));
                cfg.ExecutionExtensionsConfiguration().EnableStepDecorator(() => new MyCapturingDecorator("step-global"));
            });
            var runner = featureRunner.GetBddRunner(this);

            runner
                .Test()
                .TestScenario(
                    Some_step1,
                    Some_step2);

            var scenarios = featureRunner.GetFeatureResult().GetScenarios().ToArray();
            Assert.That(scenarios.Length, Is.EqualTo(1));

            Assert.That(CapturedMessages, Is.EqualTo(new[]
            {
                "scenario-global: It should call global and local extensions in order when scenario is executed",
                "local1: It should call global and local extensions in order when scenario is executed",
                "local2: It should call global and local extensions in order when scenario is executed",
                "step-global: Some step1",
                "s1-ext1: Some step1",
                "s1-ext2: Some step1",
                "step-global: Some step2",
                "s2-ext1: Some step2",
                "s2-ext2: Some step2"
            }));
        }

        [Test]
        [MyThrowingDecorator(ExecutionStatus.Failed)]
        public void It_should_FAIL_scenario_based_on_scenario_extension_attribute()
        {
            var featureRunner = CreateRunner(cfg => { });
            try
            {
                featureRunner
                    .GetBddRunner(this)
                    .Test()
                    .TestScenario(Some_step1);
            }
            catch { }
            var scenario = featureRunner.GetFeatureResult().GetScenarios().Single();

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario: failure"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        [Test]
        [MyThrowingDecorator(ExecutionStatus.Ignored)]
        public void It_should_IGNORE_scenario_based_on_scenario_extension_attribute()
        {
            var featureRunner = CreateRunner(cfg => { });
            try
            {
                featureRunner
                    .GetBddRunner(this)
                    .Test()
                    .TestScenario(Some_step1);
            }
            catch { }
            var scenario = featureRunner.GetFeatureResult().GetScenarios().Single();

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Ignored));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario: ignore"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        [Test]
        [MyThrowingDecorator(ExecutionStatus.Bypassed)]
        public void It_should_BYPASSED_scenario_based_on_scenario_extension_attribute()
        {
            var featureRunner = CreateRunner(cfg => { });
            featureRunner
                .GetBddRunner(this)
                .Test()
                .TestScenario(Some_step1);

            var scenario = featureRunner.GetFeatureResult().GetScenarios().Single();

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Scenario: bypassed"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.NotRun));
        }

        [Test]
        public void It_should_FAIL_scenario_based_on_step_extension_attribute()
        {
            var featureRunner = CreateRunner(cfg => { });
            try
            {
                featureRunner
                    .GetBddRunner(this)
                    .Test()
                    .TestScenario(My_failed_step);
            }
            catch { }
            var scenario = featureRunner.GetFeatureResult().GetScenarios().Single();

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Failed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1: failure"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Failed));
        }

        [Test]
        public void It_should_propagate_exception_thrown_from_step_extension_with_simple_stack_trace()
        {
            var featureRunner = CreateRunner(cfg => { });
            var ex = Assert.Throws<InvalidOperationException>(() => featureRunner
                .GetBddRunner(this)
                .Test()
                .TestScenario(My_failed_step));

            ex.AssertStackTraceMatching(@"^\s*at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.ProcessStatus\(\)[^\n]*
\s*at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.ExecuteAsync\(IStep step, Func`1 stepInvocation\)[^\n]*
\s*at LightBDD.Core.Extensibility.Execution.Implementation.DecoratingExecutor.RecursiveExecutor`1.<ExecuteAsync>[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.UnitTests.Helpers.TestableIntegration.TestSyntaxRunner.TestScenario\(IEnumerable`1 steps\)[^\n]*");
        }

        [Test]
        public void It_should_propagate_exception_thrown_from_async_step_extension_with_simple_stack_trace()
        {
            var featureRunner = CreateRunner(cfg => { });
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => featureRunner
                .GetBddRunner(this)
                .Test()
                .TestScenarioAsync(My_failed_async_step));

            ex.AssertStackTraceMatching(@"^\s*at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.ProcessStatus\(\)[^\n]*
\s*at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.<ProcessStatusAsync>[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.UnitTests.Helpers.TestableIntegration.TestSyntaxRunner.<TestScenarioAsync>[^\n]*");
        }

        [Test]
        [MyThrowingDecorator(ExecutionStatus.Failed)]
        public void It_should_propagate_exception_thrown_from_scenario_extension_with_simple_stack_trace()
        {
            var featureRunner = CreateRunner(cfg => { });
            var ex = Assert.Throws<InvalidOperationException>(() => featureRunner
                .GetBddRunner(this)
                .Test()
                .TestScenario(Some_step1));

            ex.AssertStackTraceMatching(@"^\s*at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.ProcessStatus\(\)[^\n]*
   at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.ExecuteAsync\(IScenario scenario, Func`1 scenarioInvocation\)[^\n]*
   at LightBDD.Core.Extensibility.Execution.Implementation.DecoratingExecutor.RecursiveExecutor`1.<ExecuteAsync>[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.UnitTests.Helpers.TestableIntegration.TestSyntaxRunner.TestScenario\(IEnumerable`1 steps\)[^\n]*");
        }

        [Test]
        [MyThrowingDecorator(ExecutionStatus.Failed, true)]
        public void It_should_propagate_exception_thrown_from_async_scenario_extension_with_simple_stack_trace()
        {
            var featureRunner = CreateRunner(cfg => { });
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => featureRunner
                .GetBddRunner(this)
                .Test()
                .TestScenarioAsync(Some_async_step));

            ex.AssertStackTraceMatching(@"^\s*at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.ProcessStatus\(\)[^\n]*
\s*at LightBDD.Core.UnitTests.CoreBddRunner_execution_extension_tests.MyThrowingDecorator.<ProcessStatusAsync>[^\n]*
--- End of stack trace from previous location where exception was thrown ---
([^\n]*
)?\s*at LightBDD.UnitTests.Helpers.TestableIntegration.TestSyntaxRunner.<TestScenarioAsync>[^\n]*");
        }

        [Test]
        public void It_should_IGNORE_scenario_based_on_step_extension_attribute()
        {
            var featureRunner = CreateRunner(cfg => { });
            try
            {
                featureRunner
                    .GetBddRunner(this)
                    .Test()
                    .TestScenario(My_ignored_step);
            }
            catch { }
            var scenario = featureRunner.GetFeatureResult().GetScenarios().Single();

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Ignored));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1: ignore"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Ignored));
        }

        [Test]
        public void It_should_BYPASSED_scenario_based_on_step_extension_attribute()
        {
            var featureRunner = CreateRunner(cfg => { });
            try
            {
                featureRunner
                    .GetBddRunner(this)
                    .Test()
                    .TestScenario(My_bypassed_step);
            }
            catch { }
            var scenario = featureRunner.GetFeatureResult().GetScenarios().Single();

            Assert.That(scenario.Status, Is.EqualTo(ExecutionStatus.Bypassed));
            Assert.That(scenario.StatusDetails, Is.EqualTo("Step 1: bypassed"));
            Assert.That(scenario.ExecutionTime, Is.Not.Null);
            Assert.That(scenario.GetSteps().Single().Status, Is.EqualTo(ExecutionStatus.Bypassed));
        }

        [MyThrowingDecorator(ExecutionStatus.Failed)]
        private void My_failed_step() { }

        [MyThrowingDecorator(ExecutionStatus.Failed, true)]
        private async Task My_failed_async_step()
        {
            await Task.Yield();
        }

        [MyThrowingDecorator(ExecutionStatus.Ignored)]
        private void My_ignored_step() { }

        [MyThrowingDecorator(ExecutionStatus.Bypassed)]
        private void My_bypassed_step() { }

        [MyCapturingDecorator("s1-ext1", Order = 0)]
        [MyCapturingDecorator("s1-ext2", Order = 1)]
        private void Some_step1()
        {
        }

        private async Task Some_async_step()
        {
            await Task.Yield();
        }

        [MyCapturingDecorator("s2-ext1", Order = 0)]
        [MyCapturingDecorator("s2-ext2", Order = 1)]
        private void Some_step2()
        {
        }

        private IFeatureRunner CreateRunner(Action<LightBddConfiguration> cfg)
        {
            return new TestableFeatureRunnerRepository(TestableIntegrationContextBuilder.Default().WithConfiguration(cfg)).GetRunnerFor(GetType());
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
                CapturedMessages.Add(_prefix + ": " + scenario.Info.Name.ToString());
                return scenarioInvocation.Invoke();
            }

            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                CapturedMessages.Add(_prefix + ": " + step.Info.Name.ToString());
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
                        return Task.FromResult(0);
                    case ExecutionStatus.Failed:
                        throw new InvalidOperationException("failure");
                    case ExecutionStatus.Bypassed:
                        throw new StepBypassException("bypassed");
                    case ExecutionStatus.Ignored:
                        throw new CustomIgnoreException("ignore");
                    default:
                        throw new NotImplementedException();
                }
            }

            public int Order { get; set; }
        }
    }
}
