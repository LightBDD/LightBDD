using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Implementation
{
    [DebuggerStepThrough]
    internal class BddRunner : IBddRunner, IEnrichableFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BddRunner(IEnrichableFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }
        [Obsolete("Use " + nameof(IEnrichableFeatureFixtureRunner), true)]
        public BddRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public IScenarioRunner NewScenario() => _coreRunner.NewScenario();

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IntegrationContext, TEnrichedRunner> runnerFactory) => _coreRunner.AsEnrichable().Enrich(runnerFactory);
    }
}
