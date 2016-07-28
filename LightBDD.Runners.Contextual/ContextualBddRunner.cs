using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;

namespace LightBDD
{
    internal class ContextualBddRunner<TContext>:IBddRunner<TContext>,ICoreBddRunner
    {
        private readonly ICoreBddRunner _coreRunner;
        private readonly Func<object> _contextProvider;

        public ContextualBddRunner(IBddRunner inner, Func<object> contextProvider)
        {
            _contextProvider = contextProvider;
            _coreRunner = inner.Integrate();
        }

        public void Dispose()
        {
            _coreRunner.Dispose();
        }

        public IIntegrationContext IntegrationContext => _coreRunner.IntegrationContext;

        public IFeatureResult GetFeatureResult() => _coreRunner.GetFeatureResult();

        public IScenarioRunner NewScenario()
        {
            return _coreRunner.NewScenario().WithContext(_contextProvider);
        }

        public IBddRunner AsBddRunner() => _coreRunner.AsBddRunner();
    }
}