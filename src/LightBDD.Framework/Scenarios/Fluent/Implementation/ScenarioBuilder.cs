using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Fluent.Implementation
{
    [DebuggerStepThrough]
    internal class ScenarioBuilder<TContext> : IScenarioBuilder<TContext>, IIntegrableStepGroupBuilder
    {
        private readonly IFeatureFixtureRunner _runner;
        private static readonly IEnumerable<StepDescriptor> EmptySteps = Enumerable.Empty<StepDescriptor>();
        private IEnumerable<StepDescriptor> _steps = EmptySteps;

        public ScenarioBuilder(IBddRunner<TContext> runner)
        {
            _runner = runner.Integrate();
        }

        public async Task RunAsync()
        {
            try
            {
                await _runner
                    .NewScenario()
                    .WithCapturedScenarioDetails()
                    .WithSteps(_steps)
                    .RunScenarioAsync();
            }
            catch (ScenarioExecutionException e)
            {
                e.GetOriginal().Throw();
            }
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));
            _steps = _steps.Concat(steps);
            return this;
        }

        public LightBddConfiguration Configuration => _runner.Configuration;
    }
}