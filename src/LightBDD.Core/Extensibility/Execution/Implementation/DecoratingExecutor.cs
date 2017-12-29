using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation;

namespace LightBDD.Core.Extensibility.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class DecoratingExecutor
    {
        private readonly Func<IScenario, Func<Task>, Task>[] _scenarioExecutors;
        private readonly Func<IStep, Func<Task>, Task>[] _stepExecutors;

        public DecoratingExecutor(IExecutionExtensions extensions)
        {
            _scenarioExecutors = ToInvocations(extensions.ScenarioDecorators).ToArray();
            _stepExecutors = ToInvocations(extensions.StepDecorators).ToArray();
        }

        private static IEnumerable<Func<IStep, Func<Task>, Task>> ToInvocations(IEnumerable<IStepDecorator> extensions)
        {
            return extensions.Select(e => (Func<IStep, Func<Task>, Task>)e.ExecuteAsync);
        }

        private static IEnumerable<Func<IScenario, Func<Task>, Task>> ToInvocations(IEnumerable<IScenarioDecorator> extensions)
        {
            return extensions.Select(e => (Func<IScenario, Func<Task>, Task>)e.ExecuteAsync);
        }

        public Task ExecuteScenarioAsync(IScenario scenario, Func<Task> scenarioInvocation, IEnumerable<IScenarioDecorator> scenarioDecorators)
        {
            return new RecursiveExecutor<IScenario>(_scenarioExecutors.Concat(ToInvocations(scenarioDecorators)), scenario, scenarioInvocation).ExecuteAsync();
        }

        public Task ExecuteStepAsync(IStep step, Func<Task> stepInvocation, IEnumerable<IStepDecorator> stepDecorators)
        {
            return new RecursiveExecutor<IStep>(_stepExecutors.Concat(ToInvocations(stepDecorators)), step, stepInvocation).ExecuteAsync();
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

            public async Task ExecuteAsync()
            {
                try
                {
                    var task = _enumerator.MoveNext()
                        ? _enumerator.Current.Invoke(_target, ExecuteAsync)
                        : _targetInvocation.Invoke();

                    await ScenarioExecutionFlow.WrapScenarioExceptions(task);
                }
                catch (ScenarioExecutionException)
                {
                    throw;
                }
                catch (StepExecutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ScenarioExecutionException(ex);
                }
            }
        }
    }
}