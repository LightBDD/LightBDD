using System;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class FeatureFixtureRunner : IFeatureFixtureRunner
    {
        private readonly object _fixture;
        private readonly Func<object, ICoreScenarioBuilder> _scenarioRunnerProvider;

        public FeatureFixtureRunner(object fixture, Func<object, ICoreScenarioBuilder> scenarioRunnerProvider)
        {
            _fixture = fixture;
            _scenarioRunnerProvider = scenarioRunnerProvider;
        }

        public ICoreScenarioBuilder NewScenario()
        {
            return _scenarioRunnerProvider.Invoke(_fixture);
        }
    }
}