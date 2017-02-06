using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Implementation
{
    internal class BddRunner : IBddRunner,IFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BddRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public IScenarioRunner NewScenario()
        {
            return _coreRunner.NewScenario();
        }
    }
}
