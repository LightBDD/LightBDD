using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Extended.Implementation
{
    [DebuggerStepThrough]
    internal class ExtendedScenarioRunnerFactory<TContext>
    {
        private readonly IFeatureFixtureRunner _runner;
        private readonly ExtendedStepCompiler<TContext> _stepCompiler;

        public ExtendedScenarioRunnerFactory(IFeatureFixtureRunner runner)
        {
            _runner = runner;
            _stepCompiler = new ExtendedStepCompiler<TContext>(runner.Configuration);
        }


        public IScenarioRunner BuildScenario(params Expression<Action<TContext>>[] steps)
        {
            return _runner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(_stepCompiler.ToStep));
        }

        public IScenarioRunner BuildAsyncScenario(params Expression<Func<TContext, Task>>[] steps)
        {
            return _runner
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(_stepCompiler.ToStep));
        }
    }
}