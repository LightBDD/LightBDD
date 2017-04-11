using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Implementation
{
    [DebuggerStepThrough]
    internal class BddRunner : IBddRunner, IEnrichableFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BddRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public IScenarioRunner NewScenario() => _coreRunner.NewScenario();

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, IIntegrationContext, TEnrichedRunner> runnerFactory) => _coreRunner.AsEnrichable().Enrich(runnerFactory);
    }
}
