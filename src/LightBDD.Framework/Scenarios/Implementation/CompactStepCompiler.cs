using System;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;

namespace LightBDD.Framework.Scenarios.Implementation
{
    internal static class CompactStepCompiler
    {
        public static StepDescriptor ToAsynchronousStep<TContext>(string name, Func<TContext, Task> step)
        {
            return new StepDescriptor(name, new AsyncStepExecutor<TContext>(step).ExecuteAsync);
        }

        public static StepDescriptor ToSynchronousStep<TContext>(string name, Action<TContext> step)
        {
            return new StepDescriptor(name, new StepExecutor<TContext>(step).Execute);
        }
        private class AsyncStepExecutor<TContext>
        {
            private static readonly MethodInfo AsCompositeStepMethod = ((Func<Task, Task<IStepResultDescriptor>>)AsCompositeStep<IStepResultDescriptor>).GetMethodInfo().GetGenericMethodDefinition();
            private readonly Func<TContext,Task> _invocation;

            public AsyncStepExecutor(Func<TContext, Task> invocation)
            {
                _invocation = invocation;
            }

            public async Task<IStepResultDescriptor> ExecuteAsync(object context, object[] args)
            {
                var task = _invocation.Invoke((TContext)context);
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
        private class StepExecutor<TContext>
        {
            private readonly Action<TContext> _invocation;

            public StepExecutor(Action<TContext> invocation)
            {
                _invocation = invocation;
            }
            public Task<IStepResultDescriptor> Execute(object context, object[] args)
            {
                try
                {
                    _invocation.Invoke((TContext) context);
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