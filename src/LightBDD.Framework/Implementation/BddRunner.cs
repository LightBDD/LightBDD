using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.Implementation
{
    internal class BddRunner: IBddRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BddRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public IIntegratedScenarioBuilder<NoContext> Integrate() => new IntegratedScenarioBuilder<NoContext>(_coreRunner.NewScenario());
    }
}
