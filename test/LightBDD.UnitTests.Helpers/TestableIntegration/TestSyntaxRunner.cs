using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestSyntaxRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;
        private Func<object> _contextProvider;

        public TestSyntaxRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public void TestScenario(params Action[] steps)
        {
            TestScenario(steps.Select(TestStep.CreateAsync).ToArray());
        }

        public void TestGroupScenario(params Func<TestCompositeStep>[] steps)
        {
            TestScenario(steps.Select(TestStep.CreateForGroup).ToArray());
        }

        public Task TestScenarioAsync(params Func<Task>[] steps)
        {
            return TestScenarioAsync(steps.Select(TestStep.Create).ToArray());
        }

        public void TestScenarioPurelySync(params Action[] steps)
        {
            TestScenarioPurelySync(steps.Select(TestStep.CreateSync).ToArray());
        }

        public TestSyntaxRunner WithContext(Func<object> contextProvider)
        {
            _contextProvider = contextProvider;
            return this;
        }

        public void TestScenario(params StepDescriptor[] steps)
        {
            TestScenario(steps.AsEnumerable());
        }

        public void TestScenario(IEnumerable<StepDescriptor> steps)
        {
            NewScenario()
                  .WithCapturedScenarioDetails()
                  .WithSteps(steps)
                  .RunAsynchronously()
                  .GetAwaiter()
                  .GetResult();
        }
        public Task TestScenarioAsync(params StepDescriptor[] steps)
        {
            return NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps)
                .RunAsynchronously();
        }

        private IScenarioRunner NewScenario()
        {
            var scenarioRunner = _coreRunner.NewScenario();
            return _contextProvider != null
                ? scenarioRunner.WithContext(_contextProvider)
                : scenarioRunner;
        }

        public void TestScenarioPurelySync(params StepDescriptor[] steps)
        {
            NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps)
                .RunSynchronously();
        }

        public void TestNamedScenario(string name, params StepDescriptor[] steps)
        {
            TestNamedScenarioAsync(name, steps)
                .GetAwaiter()
                .GetResult();
        }

        public Task TestNamedScenarioAsync(string name, params StepDescriptor[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithName(name)
                .WithSteps(steps)
                .RunAsynchronously();
        }
    }
}