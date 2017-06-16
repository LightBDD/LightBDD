using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Basic.Implementation
{
    [DebuggerStepThrough]
    internal class BasicScenarioRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BasicScenarioRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public void RunScenario(params Action[] steps)
        {
            _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(BasicStepCompiler.ToSynchronousStep))
                .RunSynchronously();
        }

        public Task RunScenarioAsync(params Func<Task>[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(BasicStepCompiler.ToAsynchronousStep))
                .RunAsynchronously();
        }

        public Task RunScenarioAsync(params Action[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(BasicStepCompiler.ToSynchronousStep))
                .RunAsynchronously();
        }
    }
}