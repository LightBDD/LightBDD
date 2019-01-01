using LightBDD.Core.Extensibility;
using LightBDD.Framework.Scenarios;
using System.Diagnostics;

namespace LightBDD.Framework.Implementation
{
    [DebuggerStepThrough]
    internal class BddRunner: IBddRunner
    {
        private readonly IFeatureFixtureRunner _coreRunner;

        public BddRunner(IFeatureFixtureRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }
        //TODO: fix WithCapturedScenarioDetails
        public IIntegratedScenarioBuilder<NoContext> Integrate() => new IntegratedScenarioBuilder<NoContext>(_coreRunner.NewScenario().WithCapturedScenarioDetails());
    }
}
