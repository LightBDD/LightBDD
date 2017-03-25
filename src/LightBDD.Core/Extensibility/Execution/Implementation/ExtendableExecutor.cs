using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class ExtendableExecutor
    {
        private readonly Func<IScenarioInfo, Func<Task>, Task>[] _scenarioExecutors;
        private readonly Func<IStep, Func<Task>, Task>[] _stepExecutors;

        public ExtendableExecutor(IExecutionExtensions extensions)
        {
            _scenarioExecutors = extensions.ScenarioExecutionExtensions.Select(e => (Func<IScenarioInfo, Func<Task>, Task>)e.ExecuteAsync).ToArray();
            _stepExecutors = extensions.StepExecutionExtensions.Select(e => (Func<IStep, Func<Task>, Task>)e.ExecuteAsync).ToArray();
        }

        public Task ExecuteScenarioAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation)
        {
            return new RecursiveExecutor<IScenarioInfo>(_scenarioExecutors, scenario, scenarioInvocation).ExecuteAsync();
        }

        public Task ExecuteStepAsync(IStep step, Func<Task> stepInvocation)
        {
            return new RecursiveExecutor<IStep>(_stepExecutors, step, stepInvocation).ExecuteAsync();
        }

        [DebuggerStepThrough]
        private class RecursiveExecutor<T>
        {
            private readonly Func<T, Func<Task>, Task>[] _extensions;
            private readonly T _target;
            private readonly Func<Task> _targetInvocation;
            private int _index;

            public RecursiveExecutor(Func<T, Func<Task>, Task>[] extensions, T target, Func<Task> targetInvocation)
            {
                _extensions = extensions;
                _target = target;
                _targetInvocation = targetInvocation;
                _index = 0;
            }

            public Task ExecuteAsync()
            {
                var index = _index++;
                return index < _extensions.Length
                    ? _extensions[index].Invoke(_target, ExecuteAsync)
                    : _targetInvocation.Invoke();
            }
        }
    }
}