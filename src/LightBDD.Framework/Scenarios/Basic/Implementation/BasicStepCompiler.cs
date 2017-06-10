using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Basic.Implementation
{
    [DebuggerStepThrough]
    internal static class BasicStepCompiler
    {
        public static StepDescriptor ToAsynchronousStep(Func<Task> step)
        {
            return new StepDescriptor(step.GetMethodInfo().Name, new AsyncStepExecutor(step).ExecuteAsync);
        }

        public static StepDescriptor ToSynchronousStep(Action step)
        {
            return new StepDescriptor(step.GetMethodInfo().Name, new StepExecutor(step).Execute);
        }

        [DebuggerStepThrough]
        private class AsyncStepExecutor
        {
            private static readonly MethodInfo AsCompositeStepMethod = ((Func<Task, Task<StepResultDescriptor>>)AsCompositeStep<StepResultDescriptor>).GetMethodInfo().GetGenericMethodDefinition();
            private readonly Func<Task> _invocation;

            public AsyncStepExecutor(Func<Task> invocation)
            {
                _invocation = invocation;
            }

            public async Task<StepResultDescriptor> ExecuteAsync(object context, object[] args)
            {
                var task = _invocation.Invoke();
                await task;

                if (IsCompositeStepTask(task))
                    return await ConvertToCompositeStep(task);

                return StepResultDescriptor.Default;
            }

            private static Task<StepResultDescriptor> ConvertToCompositeStep(Task task)
            {
                return (Task<StepResultDescriptor>)AsCompositeStepMethod
                    .MakeGenericMethod(task.GetType().GetTypeInfo().GenericTypeArguments)
                    .Invoke(null, new object[] { task });
            }

            private static bool IsCompositeStepTask(Task task)
            {
                var taskType = task.GetType().GetTypeInfo();
                if (!taskType.IsGenericType)
                    return false;
                return taskType.GenericTypeArguments.Length == 1
                    && typeof(StepResultDescriptor).GetTypeInfo().IsAssignableFrom(taskType.GenericTypeArguments[0].GetTypeInfo());
            }

            private static async Task<StepResultDescriptor> AsCompositeStep<T>(Task stepTask) where T : StepResultDescriptor
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
            public Task<StepResultDescriptor> Execute(object context, object[] args)
            {
                _invocation.Invoke();
                return Task.FromResult(StepResultDescriptor.Default);
            }
        }
    }
}