using System;
using System.Diagnostics;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class FeatureFixtureRunner : IEnrichableFeatureFixtureRunner
    {
        private readonly object _fixture;
        private readonly Func<object, IScenarioRunner> _scenarioRunnerProvider;
        private readonly IntegrationContext _integrationContext;

        public FeatureFixtureRunner(object fixture, Func<object, IScenarioRunner> scenarioRunnerProvider, IntegrationContext integrationContext)
        {
            _fixture = fixture;
            _scenarioRunnerProvider = scenarioRunnerProvider;
            _integrationContext = integrationContext;
        }

        public IScenarioRunner NewScenario() => _scenarioRunnerProvider.Invoke(_fixture);
        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IntegrationContext, TEnrichedRunner> runnerFactory)
        {
            if (runnerFactory == null)
                throw new ArgumentNullException(nameof(runnerFactory));
            return runnerFactory(this, _integrationContext);
        }
    }
}