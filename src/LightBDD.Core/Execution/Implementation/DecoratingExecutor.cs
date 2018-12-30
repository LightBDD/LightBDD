using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Core.Execution.Implementation
{
    [DebuggerStepThrough]
    internal class DecoratingExecutor
    {
        private static IEnumerable<Func<IStep, Func<Task>, Task>> ToInvocations(IEnumerable<IStepDecorator> extensions)
        {
            return extensions.Select(e => (Func<IStep, Func<Task>, Task>)e.ExecuteAsync);
        }

        private static IEnumerable<Func<IScenario, Func<Task>, Task>> ToInvocations(IEnumerable<IScenarioDecorator> extensions)
        {
            return extensions.Select(e => (Func<IScenario, Func<Task>, Task>)e.ExecuteAsync);
        }

        public static Func<Task> DecorateScenario(IScenario scenario, Func<Task> scenarioInvocation, IEnumerable<IScenarioDecorator> scenarioDecorators)
        {
            return new DecoratingExecutor<IScenario>(ToInvocations(scenarioDecorators), scenario, scenarioInvocation).ExecuteAsync;
        }

        public static Func<Task> DecorateStep(IStep step, Func<Task> stepInvocation, IEnumerable<IStepDecorator> stepDecorators)
        {
            return new DecoratingExecutor<IStep>(ToInvocations(stepDecorators), step, stepInvocation).ExecuteAsync;
        }
    }

    [DebuggerStepThrough]
    internal class DecoratingExecutor<T>
    {
        private readonly IEnumerator<Func<T, Func<Task>, Task>> _enumerator;
        private readonly T _target;
        private readonly Func<Task> _targetInvocation;

        public DecoratingExecutor(IEnumerable<Func<T, Func<Task>, Task>> extensions, T target, Func<Task> targetInvocation)
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
            catch (Exception ex)
            {
                if (ScenarioExecutionException.TryWrap(ex, out var wrapped))
                    throw wrapped;
                throw;
            }
        }
    }
}