using System;
using System.Diagnostics;

namespace LightBDD.Core.Extensibility.Implementation
{
    [DebuggerStepThrough]
    internal class FeatureFixtureRunner : IFeatureFixtureRunner
    {
        private readonly object _fixture;
        private readonly Func<object, IScenarioRunner> _scenarioRunnerProvider;

        public FeatureFixtureRunner(object fixture, Func<object, IScenarioRunner> scenarioRunnerProvider)
        {
            _fixture = fixture;
            _scenarioRunnerProvider = scenarioRunnerProvider;
        }

        public IScenarioRunner NewScenario() => _scenarioRunnerProvider.Invoke(_fixture);
    }
}