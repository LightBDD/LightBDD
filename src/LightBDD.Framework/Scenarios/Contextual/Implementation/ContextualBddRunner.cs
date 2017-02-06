using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>, IFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;
        private readonly Func<object> _contextProvider;

        public ContextualBddRunner(IBddRunner inner, Func<object> contextProvider)
        {
            _contextProvider = contextProvider;
            _coreRunner = inner.Integrate();
        }

        public IScenarioRunner NewScenario()
        {
            return _coreRunner.NewScenario().WithContext(_contextProvider);
        }
    }
}