using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
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

        public IScenarioRunner NewScenario()
        {
            return _coreRunner.NewScenario();
        }

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, LightBddConfiguration, TEnrichedRunner> runnerFactory)
        {
            return _coreRunner.AsEnrichable().Enrich(runnerFactory);
        }
    }
}
