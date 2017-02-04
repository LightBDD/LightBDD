using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Implementation
{
    internal class BddRunner : IBddRunner,ICoreBddRunner
    {
        private readonly ICoreBddRunner _coreRunner;

        public BddRunner(ICoreBddRunner coreRunner)
        {
            _coreRunner = coreRunner;
        }

        public IScenarioRunner NewScenario()
        {
            return _coreRunner.NewScenario();
        }
    }
}
