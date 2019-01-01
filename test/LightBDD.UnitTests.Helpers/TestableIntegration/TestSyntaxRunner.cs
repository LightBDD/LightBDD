using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.Framework.Scenarios.Contextual;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestSyntaxRunner
    {
        private readonly IBddRunner _coreRunner;
        private Func<object> _contextProvider;
        private bool _takeContextOwnership;

        public TestSyntaxRunner(IBddRunner runner)
        {
            _coreRunner = runner;
        }

        public void TestScenario(params Action[] steps)
        {
            TestScenario(steps.Select(TestStep.CreateAsync).ToArray());
        }

        public void TestGroupScenario(params Func<TestCompositeStep>[] steps)
        {
            TestScenario(steps.Select(TestStep.CreateComposite).ToArray());
        }

        public Task TestScenarioAsync(params Func<Task>[] steps)
        {
            return TestScenarioAsync(steps.Select(TestStep.Create).ToArray());
        }

        public void TestScenarioPurelySync(params Action[] steps)
        {
            TestScenarioPurelySync(steps.Select(TestStep.CreateSync).ToArray());
        }

        public TestSyntaxRunner WithContext(Func<object> contextProvider, bool takeOwnership)
        {
            _takeContextOwnership = takeOwnership;
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
                    .AddSteps(steps)
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
                    .AddSteps(steps)
                    .RunScenarioAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        private ICoreScenarioBuilder NewScenario()
        {
            var scenarioRunner = _coreRunner.Integrate().Core;
            return _contextProvider != null
                ? scenarioRunner.WithContext(_contextProvider, _takeContextOwnership)
                : scenarioRunner;
        }

        public void TestScenarioPurelySync(params StepDescriptor[] steps)
        {
            try
            {
                NewScenario()
                    .WithCapturedScenarioDetails()
                    .AddSteps(steps)
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
                    .Integrate()
                    .Core
                    .WithName(name)
                    .AddSteps(steps)
                    .RunScenarioAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
    }
}