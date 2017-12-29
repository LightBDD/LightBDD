using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;

namespace LightBDD.Framework.Scenarios.Basic.Implementation
{
    [DebuggerStepThrough]
    internal static class BasicStepCompiler
    {
        public static StepDescriptor ToAsynchronousStep(Func<Task> step)
        {
            return new StepDescriptor(step.GetMethodInfo(), new AsyncStepExecutor(step).ExecuteAsync);
        }

        public static StepDescriptor ToSynchronousStep(Action step)
        {
            return new StepDescriptor(step.GetMethodInfo(), new StepExecutor(step).Execute);
        }

        [DebuggerStepThrough]
        private class AsyncStepExecutor
        {
            private static readonly MethodInfo AsCompositeStepMethod = ((Func<Task, Task<IStepResultDescriptor>>)AsCompositeStep<IStepResultDescriptor>).GetMethodInfo().GetGenericMethodDefinition();
            private readonly Func<Task> _invocation;

            public AsyncStepExecutor(Func<Task> invocation)
            {
                _invocation = invocation;
            }

            public async Task<IStepResultDescriptor> ExecuteAsync(object context, object[] args)
            {
                var task = _invocation.Invoke();
                await ScenarioExecutionFlow.WrapScenarioExceptions(task);

                if (HasResultDescriptor(task))
                    return await ConvertToResultDescriptor(task);

                return DefaultStepResultDescriptor.Instance;
            }

            private static Task<IStepResultDescriptor> ConvertToResultDescriptor(Task task)
            {
                return (Task<IStepResultDescriptor>)AsCompositeStepMethod
                    .MakeGenericMethod(task.GetType().GetTypeInfo().GenericTypeArguments)
                    .Invoke(null, new object[] { task });
            }

            private static bool HasResultDescriptor(Task task)
            {
                var taskType = task.GetType().GetTypeInfo();
                if (!taskType.IsGenericType)
                    return false;
                return taskType.GenericTypeArguments.Length == 1
                    && typeof(IStepResultDescriptor).GetTypeInfo().IsAssignableFrom(taskType.GenericTypeArguments[0].GetTypeInfo());
            }

            private static async Task<IStepResultDescriptor> AsCompositeStep<T>(Task stepTask) where T : IStepResultDescriptor
            {
                return await (Task<T>)stepTask;
            }
        }

        [DebuggerStepThrough]
        private class StepExecutor
        {
            private readonly Action _invocation;

            public StepExecutor(Action invocation)
            {
                _invocation = invocation;
            }
            public Task<IStepResultDescriptor> Execute(object context, object[] args)
            {
                try
                {
                    _invocation.Invoke();
                    return Task.FromResult(DefaultStepResultDescriptor.Instance);
                }
                catch (Exception e)
                {
                    throw new ScenarioExecutionException(e);
                }
            }
        }
    }
}