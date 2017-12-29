using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
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
            try
            {
                NewScenario()
                    .WithCapturedScenarioDetails()
                    .WithSteps(steps)
                    .RunScenarioAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
        public async Task TestScenarioAsync(params StepDescriptor[] steps)
        {
            try
            {
                await NewScenario()
                    .WithCapturedScenarioDetails()
                    .WithSteps(steps)
                    .RunScenarioAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
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
            try
            {
                NewScenario()
                    .WithCapturedScenarioDetails()
                    .WithSteps(steps)
                    .RunScenario();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        public void TestNamedScenario(string name, params StepDescriptor[] steps)
        {
            TestNamedScenarioAsync(name, steps)
                .GetAwaiter()
                .GetResult();
        }

        public async Task TestNamedScenarioAsync(string name, params StepDescriptor[] steps)
        {
            try
            {
                await _coreRunner
                    .NewScenario()
                    .WithName(name)
                    .WithSteps(steps)
                    .RunScenarioAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
    }
}