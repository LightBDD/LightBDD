using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Configuration;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Extensibility.Implementation
{
    internal class ExtendableExecutor : IExtendableExecutor
    {
        private readonly IScenarioExecutionExtension[] _scenarios;
        private readonly IStepExecutionExtension[] _steps;

        public ExtendableExecutor(ExecutionExtensionsConfiguration extensions)
        {
            _scenarios = extensions.ScenarioExecutionExtensions.ToArray();
            _steps = extensions.StepExecutionExtensions.ToArray();
        }

        public Task ExecuteScenario(IScenarioInfo scenario, Func<Task> scenarioInvocation)
        {
            return ExecuteScenario(scenario, scenarioInvocation, 0).Invoke();
        }

        public Task ExecuteStep(IStep step, Func<Task> stepInvocation)
        {
            return ExecuteStep(step, stepInvocation, 0).Invoke();
        }

        private Func<Task> ExecuteScenario(IScenarioInfo scenario, Func<Task> scenarioInvocation, int index)
        {
            return index == _scenarios.Length
                ? scenarioInvocation
                : () => _scenarios[index].ExecuteAsync(scenario, ExecuteScenario(scenario, scenarioInvocation, index + 1));
        }

        private Func<Task> ExecuteStep(IStep step, Func<Task> stepInvocation, int index)
        {
            return index == _steps.Length
                ? stepInvocation
                : () => _steps[index].ExecuteAsync(step, ExecuteStep(step, stepInvocation, index + 1));
        }
    }
}