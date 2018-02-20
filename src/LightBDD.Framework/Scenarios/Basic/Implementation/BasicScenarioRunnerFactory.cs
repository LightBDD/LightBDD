using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Basic.Implementation
{
    [DebuggerStepThrough]
    internal class BasicScenarioRunnerFactory
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BasicScenarioRunnerFactory(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public IScenarioRunner BuildScenario(params Action[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(BasicStepCompiler.ToSynchronousStep));
        }

        public IScenarioRunner BuildAsyncScenario(params Func<Task>[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(BasicStepCompiler.ToAsynchronousStep));
        }

        public IScenarioRunner BuildAsyncScenario(params Action[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(BasicStepCompiler.ToSynchronousStep));
        }
    }
}