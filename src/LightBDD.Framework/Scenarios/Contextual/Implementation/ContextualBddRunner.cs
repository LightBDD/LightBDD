using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>, IEnrichableFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;
        private readonly Func<object> _contextProvider;

        public ContextualBddRunner(IBddRunner coreRunner, Func<object> contextProvider)
        {
            _contextProvider = contextProvider;
            _coreRunner = coreRunner.Integrate();
        }

        public IScenarioRunner NewScenario()
        {
            return _coreRunner.NewScenario().WithContext(_contextProvider);
        }

        public TEnrichedRunner Enrich<TEnrichedRunner>(Func<IFeatureFixtureRunner, LightBddConfiguration, TEnrichedRunner> runnerFactory)
        {
            return _coreRunner.AsEnrichable().Enrich(new ContextualRunnerEnricher<TEnrichedRunner>(this, runnerFactory).Enrich);
        }

        [DebuggerStepThrough]
        private struct ContextualRunnerEnricher<TRunner>
        {
            private readonly IFeatureFixtureRunner _contextualRunner;
            private readonly Func<IFeatureFixtureRunner, LightBddConfiguration, TRunner> _runnerFactory;

            public ContextualRunnerEnricher(IFeatureFixtureRunner contextualRunner, Func<IFeatureFixtureRunner, LightBddConfiguration, TRunner> runnerFactory)
            {
                _contextualRunner = contextualRunner;
                _runnerFactory = runnerFactory;
            }

            public TRunner Enrich(IFeatureFixtureRunner _, LightBddConfiguration ctx)
            {
                return _runnerFactory(_contextualRunner, ctx);
            }
        }
    }
}