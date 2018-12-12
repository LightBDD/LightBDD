using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Implementation
{
    [DebuggerStepThrough]
    internal class BddRunner : IBddRunner, IFeatureFixtureRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BddRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public IScenarioRunner NewScenario() => _coreRunner.NewScenario();
        public LightBddConfiguration Configuration => _coreRunner.Configuration;
    }
}
