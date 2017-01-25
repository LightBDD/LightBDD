using System;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.Execution.Implementation
{
    internal class CoreBddRunner : IBddRunner, ICoreBddRunner
    {
        private readonly Func<IScenarioRunner> _scenarioRunnerProvider;

        public CoreBddRunner(Func<IScenarioRunner> scenarioRunnerProvider)
        {
            _scenarioRunnerProvider = scenarioRunnerProvider;
        }

        public IScenarioRunner NewScenario() => _scenarioRunnerProvider.Invoke();
    }
}