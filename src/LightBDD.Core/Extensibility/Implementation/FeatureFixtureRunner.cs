using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class FeatureFixtureRunner : IEnrichableFeatureFixtureRunner
    {
        private readonly object _fixture;
        private readonly Func<object, IScenarioRunner> _scenarioRunnerProvider;
        private readonly LightBddConfiguration _configuration;

        public FeatureFixtureRunner(object fixture, Func<object, IScenarioRunner> scenarioRunnerProvider, LightBddConfiguration configuration)
        {
            _fixture = fixture;
            _scenarioRunnerProvider = scenarioRunnerProvider;
            _configuration = configuration;
        }

        public IScenarioRunner NewScenario()
        {
            return _scenarioRunnerProvider.Invoke(_fixture);
        }

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, LightBddConfiguration, TEnrichedRunner> runnerFactory)
        {
            if (runnerFactory == null)
                throw new ArgumentNullException(nameof(runnerFactory));
            return runnerFactory(this, _configuration);
        }
    }
}