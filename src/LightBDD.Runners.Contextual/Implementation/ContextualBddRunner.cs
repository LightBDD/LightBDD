using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;

namespace LightBDD.Runners.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>, ICoreBddRunner
    {
        private readonly ICoreBddRunner _coreRunner;
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