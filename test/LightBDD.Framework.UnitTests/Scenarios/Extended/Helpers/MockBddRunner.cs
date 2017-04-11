using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class MockBddRunner<T> : IBddRunner<T>, IEnrichableFeatureFixtureRunner
    {
        private readonly IIntegrationContext _ctx;
        private readonly IScenarioRunner _scenarioRunner;

        public MockBddRunner(IIntegrationContext ctx, IScenarioRunner scenarioRunner)
        {
            _ctx = ctx;
            _scenarioRunner = scenarioRunner;
        }

        public IScenarioRunner NewScenario() => _scenarioRunner;

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IIntegrationContext, TEnrichedRunner> runnerFactory) => runnerFactory(this, _ctx);
    }
}