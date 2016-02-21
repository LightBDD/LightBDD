using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    internal class TestSyntaxRunner
    {
        private readonly ICoreBddRunner _coreRunner;

        public TestSyntaxRunner(ICoreBddRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public void TestScenario(params Action[] steps)
        {
            TestScenario(steps.Select(TestStep.CreateAsync).ToArray());
        }

        public Task TestScenarioAsync(params Func<Task>[] steps)
        {
            return TestScenarioAsync(steps.Select(TestStep.Create).ToArray());
        }

        public void TestScenarioPurelySync(params Action[] steps)
        {
            TestScenarioPurelySync(steps.Select(TestStep.CreateSync).ToArray());
        }

        public void TestScenario(params StepDescriptor[] steps)
        {
            _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps)
                .RunAsynchronously()
                .GetAwaiter()
                .GetResult();
        }

        public Task TestScenarioAsync(params StepDescriptor[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps)
                .RunAsynchronously();
        }

        public void TestScenarioPurelySync(params StepDescriptor[] steps)
        {
            _coreRunner
                .NewScenario()
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