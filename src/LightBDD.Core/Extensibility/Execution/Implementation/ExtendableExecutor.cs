using System;
using System.Collections.Generic;
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
            _scenarioExecutors = ToInvocations(extensions.ScenarioExecutionExtensions).ToArray();
            _stepExecutors = ToInvocations(extensions.StepExecutionExtensions).ToArray();
        }

        private static IEnumerable<Func<IStep, Func<Task>, Task>> ToInvocations(IEnumerable<IStepExecutionExtension> extensions)
        {
            return extensions.Select(e => (Func<IStep, Func<Task>, Task>)e.ExecuteAsync);
        }

        private static IEnumerable<Func<IScenarioInfo, Func<Task>, Task>> ToInvocations(IEnumerable<IScenarioExecutionExtension> extensions)
        {
            return extensions.Select(e => (Func<IScenarioInfo, Func<Task>, Task>)e.ExecuteAsync);
        }

        public Task ExecuteScenarioAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation, IEnumerable<IScenarioExecutionExtension> scenarioExecutionExtensions)
        {
            return new RecursiveExecutor<IScenarioInfo>(_scenarioExecutors.Concat(ToInvocations(scenarioExecutionExtensions)), scenario, scenarioInvocation).ExecuteAsync();
        }

        public Task ExecuteStepAsync(IStep step, Func<Task> stepInvocation, IEnumerable<IStepExecutionExtension> stepExecutionExtensions)
        {
            return new RecursiveExecutor<IStep>(_stepExecutors.Concat(ToInvocations(stepExecutionExtensions)), step, stepInvocation).ExecuteAsync();
        }

        [DebuggerStepThrough]
        private class RecursiveExecutor<T>
        {
            private readonly IEnumerator<Func<T, Func<Task>, Task>> _enumerator;
            private readonly T _target;
            private readonly Func<Task> _targetInvocation;

            public RecursiveExecutor(IEnumerable<Func<T, Func<Task>, Task>> extensions, T target, Func<Task> targetInvocation)
            {
                _enumerator = extensions.GetEnumerator();
                _target = target;
                _targetInvocation = targetInvocation;
            }

            public Task ExecuteAsync()
            {
                return _enumerator.MoveNext()
                    ? _enumerator.Current.Invoke(_target, ExecuteAsync)
                    : _targetInvocation.Invoke();
            }
        }
    }
}