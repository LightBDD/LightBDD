using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Framework;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public class TestSyntaxRunner
    {
        private Func<object> _contextProvider;
        private bool _takeContextOwnership;
        private Func<IDependencyResolver, object> _contextResolver;
        private Func<ICoreScenarioBuilder> _integrate;
        public TestSyntaxRunner(IBddRunner runner)
        {
            _integrate = () => runner.Integrate().Core;
        }

        public TestSyntaxRunner(ICoreScenarioBuilder scenarioBuilder)
        {
            _integrate = () => scenarioBuilder;
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

        public TestSyntaxRunner WithContext(Func<IDependencyResolver, object> contextResolver)
        {
            _contextResolver = contextResolver;
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
                    .Build()
                    .ExecuteAsync()
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
                    .Build()
                    .ExecuteAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        private ICoreScenarioBuilder NewScenario()
        {
            var scenarioRunner = _integrate.Invoke();
            if (_contextProvider != null)
                return scenarioRunner.WithContext(_contextProvider, _takeContextOwnership);
            if (_contextResolver != null)
                return scenarioRunner.WithContext(_contextResolver);
            return scenarioRunner;
        }

        public void TestScenarioPurelySync(params StepDescriptor[] steps)
        {
            try
            {
                NewScenario()
                    .WithCapturedScenarioDetails()
                    .AddSteps(steps)
                    .Build()
                    .ExecuteSync();
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
                await _integrate.Invoke()
                    .WithName(name)
                    .AddSteps(steps)
                    .Build()
                    .ExecuteAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }
    }
}