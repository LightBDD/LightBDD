using System;
using System.Diagnostics;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class CoreBddRunner : ICoreBddRunner
    {
        private readonly object _fixture;
        private readonly Func<object, IScenarioRunner> _scenarioRunnerProvider;

        public CoreBddRunner(object fixture, Func<object, IScenarioRunner> scenarioRunnerProvider)
        {
            _fixture = fixture;
            _scenarioRunnerProvider = scenarioRunnerProvider;
        }

        public IScenarioRunner NewScenario() => _scenarioRunnerProvider.Invoke(_fixture);
    }
}