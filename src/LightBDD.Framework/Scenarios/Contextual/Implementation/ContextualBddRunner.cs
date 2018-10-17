using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using System;
using System.Diagnostics;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>, IEnrichableFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;
        private Action<IScenarioRunner> _configureFn = ConfigureNothing;
        private static void ConfigureNothing(IScenarioRunner runner) { }

        public ContextualBddRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public ContextualBddRunner<TContext> Configure(Action<IScenarioRunner> configureFn)
        {
            _configureFn += configureFn;
            return this;
        }

        public IScenarioRunner NewScenario()
        {
            var runner = _coreRunner.NewScenario();
            _configureFn(runner);
            return runner;
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