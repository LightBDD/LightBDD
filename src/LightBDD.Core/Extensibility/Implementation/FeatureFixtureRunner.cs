using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class FeatureFixtureRunner : IFeatureFixtureRunner
    {
        private readonly object _fixture;
        private readonly Func<object, IScenarioRunner> _scenarioRunnerProvider;

        public FeatureFixtureRunner(object fixture, Func<object, IScenarioRunner> scenarioRunnerProvider, LightBddConfiguration configuration)
        {
            Configuration = configuration;
            _fixture = fixture;
            _scenarioRunnerProvider = scenarioRunnerProvider;
        }

        public IScenarioRunner NewScenario()
        {
            return _scenarioRunnerProvider.Invoke(_fixture);
        }

        public LightBddConfiguration Configuration { get; }
    }
}