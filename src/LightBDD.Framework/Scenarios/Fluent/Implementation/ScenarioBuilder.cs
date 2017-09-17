using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Fluent.Implementation
{
    internal class ScenarioBuilder<TContext> : LightBddConfigurationAware, IScenarioBuilder<TContext>, IIntegrableStepGroupBuilder
    {
        private readonly IBddRunner<TContext> _runner;
        private static readonly IEnumerable<StepDescriptor> EmptySteps = Enumerable.Empty<StepDescriptor>();
        private IEnumerable<StepDescriptor> _steps = EmptySteps;

        public ScenarioBuilder(IBddRunner<TContext> runner)
        {
            _runner = runner;
        }

        public async Task RunAsync()
        {
            await _runner.Integrate()
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(_steps)
                .RunAsynchronously();
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            if (steps == null)
                throw new ArgumentNullException(nameof(steps));
            _steps = _steps.Concat(steps);
            return this;
        }

        public TEnrichedBuilder Enrich<TEnrichedBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> builderFactory)
        {
            return builderFactory(this, Configuration);
        }
    }
}