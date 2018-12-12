using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    public class MockBddRunner<T> : IBddRunner<T>, IFeatureFixtureRunner
    {
        private readonly IScenarioRunner _scenarioRunner;

        public LightBddConfiguration Configuration { get; }

        public MockBddRunner(LightBddConfiguration configuration, IScenarioRunner scenarioRunner)
        {
            Configuration = configuration;
            _scenarioRunner = scenarioRunner;
        }

        public IScenarioRunner NewScenario()
        {
            return _scenarioRunner;
        }
    }
}