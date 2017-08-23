using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
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
        [MyCapturingExtension("local2", Order = 100)]
        [MyCapturingExtension("local1", Order = 0)]
        public void It_should_call_global_and_local_extensions_in_order_when_scenario_is_executed()
        {
            var featureRunner = CreateRunner(cfg =>
            {
                cfg.ExecutionExtensionsConfiguration().EnableScenarioExtension(() => new MyCapturingExtension("scenario-global"));
                cfg.ExecutionExtensionsConfiguration().EnableStepExtension(() => new MyCapturingExtension("step-global"));
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
                "s2-ext2: Some step2",
            }));
        }

        [Test]
        [MyThrowingExtension(ExecutionStatus.Failed)]
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
        [MyThrowingExtension(ExecutionStatus.Ignored)]
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
        [MyThrowingExtension(ExecutionStatus.Bypassed)]
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

        [MyThrowingExtension(ExecutionStatus.Failed)]
        private void My_failed_step() { }

        [MyThrowingExtension(ExecutionStatus.Ignored)]
        private void My_ignored_step() { }

        [MyThrowingExtension(ExecutionStatus.Bypassed)]
        private void My_bypassed_step() { }

        [MyCapturingExtension("s1-ext1", Order = 0)]
        [MyCapturingExtension("s1-ext2", Order = 1)]
        private void Some_step1()
        {
        }

        [MyCapturingExtension("s2-ext1", Order = 0)]
        [MyCapturingExtension("s2-ext2", Order = 1)]
        private void Some_step2()
        {
        }

        private IFeatureRunner CreateRunner(Action<LightBddConfiguration> cfg)
        {
            return new TestableFeatureRunnerRepository(TestableIntegrationContextBuilder.Default().WithConfiguration(cfg)).GetRunnerFor(GetType());
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        class MyCapturingExtension : Attribute, IScenarioExecutionExtensionAttribute, IStepExecutionExtensionAttribute
        {
            private readonly string _prefix;

            public MyCapturingExtension(string prefix)
            {
                _prefix = prefix;
            }

            public Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation)
            {
                CapturedMessages.Add(_prefix + ": " + scenario.Name.ToString());
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
        class MyThrowingExtension : Attribute, IScenarioExecutionExtensionAttribute, IStepExecutionExtensionAttribute
        {
            private readonly ExecutionStatus _expected;

            public MyThrowingExtension(ExecutionStatus expected)
            {
                _expected = expected;
            }

            public Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation)
            {
                return ProcessStatus();
            }

            public Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
            {
                return ProcessStatus();
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
