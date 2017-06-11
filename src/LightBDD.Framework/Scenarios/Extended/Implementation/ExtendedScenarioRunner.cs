using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Extended.Implementation
{
    [DebuggerStepThrough]
    internal class ExtendedScenarioRunner<TContext>
    {
        private readonly IFeatureFixtureRunner _runner;
        private readonly ExtendedStepCompiler<TContext> _stepCompiler;

        public ExtendedScenarioRunner(IFeatureFixtureRunner runner, LightBddConfiguration configuration)
        {
            _runner = runner;
            _stepCompiler = new ExtendedStepCompiler<TContext>(configuration);
        }


        public void RunScenario(params Expression<Action<TContext>>[] steps)
        {
            _runner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(_stepCompiler.ToStep))
                .RunSynchronously();
        }

        public Task RunScenarioAsync(params Expression<Func<TContext, Task>>[] steps)
        {
            return _runner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(_stepCompiler.ToStep))
                .RunAsynchronously();
        }

        public Task RunScenarioAsync(params Expression<Action<TContext>>[] steps)
        {
            return _runner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(_stepCompiler.ToStep))
                .RunAsynchronously();
        }

        public static ExtendedScenarioRunner<TContext> Create(IFeatureFixtureRunner runner, LightBddConfiguration configuration)
        {
            return new ExtendedScenarioRunner<TContext>(runner, configuration);
        }
    }
}