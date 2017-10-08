using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class MockBddRunner<T> : IBddRunner<T>, IEnrichableFeatureFixtureRunner
    {
        private readonly LightBddConfiguration _configuration;
        private readonly IScenarioRunner _scenarioRunner;

        public MockBddRunner(LightBddConfiguration configuration, IScenarioRunner scenarioRunner)
        {
            _configuration = configuration;
            _scenarioRunner = scenarioRunner;
        }

        public IScenarioRunner NewScenario()
        {
            return _scenarioRunner;
        }

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, LightBddConfiguration, TEnrichedRunner> runnerFactory)
        {
            return runnerFactory(this, _configuration);
        }
    }
}