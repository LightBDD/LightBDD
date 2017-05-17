using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Basic.Implementation
{
    //TODO: check return value?
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

        public Task RunScenarioAsync(params Action[] steps)
        {
            return _coreRunner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(ToSynchronousStep))
                .RunAsynchronously();
        }

        private StepDescriptor ToAsynchronousStep(Func<Task> step)
        {
            return new StepDescriptor(step.GetMethodInfo().Name, new AsyncStepExecutor(step).ExecuteAsync);
        }

        private static StepDescriptor ToSynchronousStep(Action step)
        {
            return new StepDescriptor(step.GetMethodInfo().Name, new StepExecutor(step).Execute);
        }

        [DebuggerStepThrough]
        private class AsyncStepExecutor
        {
            private readonly Func<Task> _invocation;

            public AsyncStepExecutor(Func<Task> invocation)
            {
                _invocation = invocation;
            }
            public async Task<StepResultDescriptor> ExecuteAsync(object context, object[] args)
            {
                await _invocation.Invoke();
                return StepResultDescriptor.Default;
            }
        }
        [DebuggerStepThrough]
        private class StepExecutor
        {
            private readonly Action _invocation;

            public StepExecutor(Action invocation)
            {
                _invocation = invocation;
            }
            public Task<StepResultDescriptor> Execute(object context, object[] args)
            {
                _invocation.Invoke();
                return Task.FromResult(StepResultDescriptor.Default);
            }
        }
    }
}