using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class MockBddRunner<T> : IBddRunner<T>, IEnrichableFeatureFixtureRunner
    {
        private readonly IntegrationContext _ctx;
        private readonly IScenarioRunner _scenarioRunner;

        public MockBddRunner(IntegrationContext ctx, IScenarioRunner scenarioRunner)
        {
            _ctx = ctx;
            _scenarioRunner = scenarioRunner;
        }

        public IScenarioRunner NewScenario() => _scenarioRunner;

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IntegrationContext, TEnrichedRunner> runnerFactory) => runnerFactory(this, _ctx);
    }
}