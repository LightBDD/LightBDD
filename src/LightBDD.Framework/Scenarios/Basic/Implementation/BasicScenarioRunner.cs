using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
                .WithSteps(steps.Select(ToSynchronousStep))
                .RunSynchronously();
        }

        public Task RunScenarioAsync(params Func<Task>[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(ToAsynchronousStep))
                .RunAsynchronously();
        }

        private StepDescriptor ToAsynchronousStep(Func<Task> step)
        {
            return new StepDescriptor(step.GetMethodInfo().Name, (ctx, args) => step.Invoke());
        }

        private static StepDescriptor ToSynchronousStep(Action step)
        {
            return new StepDescriptor(step.GetMethodInfo().Name, (ctx, args) =>
            {
                step.Invoke();
                return Task.FromResult(0);
            });
        }
    }
}