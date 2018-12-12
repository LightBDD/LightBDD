using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualBddRunner<TContext> : IBddRunner<TContext>, IFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;
        private readonly Func<IScenarioRunner, IScenarioRunner> _configureScenarioContext;

        public ContextualBddRunner(IBddRunner coreRunner, Func<object> contextProvider, bool takeOwnership)
        {
            _configureScenarioContext = scenario => scenario.WithContext(contextProvider, takeOwnership);
            _coreRunner = coreRunner.Integrate();
        }

        public ContextualBddRunner(IBddRunner coreRunner, Func<IDependencyResolver, object> contextResolver)
        {
            _configureScenarioContext = scenario => scenario.WithContext(contextResolver);
            _coreRunner = coreRunner.Integrate();
        }

        public IScenarioRunner NewScenario()
        {
            return _configureScenarioContext(_coreRunner.NewScenario());
        }

        public LightBddConfiguration Configuration => _coreRunner.Configuration;
    }
}