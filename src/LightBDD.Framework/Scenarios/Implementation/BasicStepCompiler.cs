using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework.Scenarios.Implementation
{
    internal static class BasicStepCompiler
    {
        public static StepDescriptor ToAsynchronousStep(Func<Task> step)
        {
            try
            {
                var methodInfo = step.GetMethodInfo();
                EnsureNotGenerated(methodInfo);
                return new StepDescriptor(methodInfo, new AsyncStepExecutor(step).ExecuteAsync);
            }
            catch (Exception ex)
            {
                return StepDescriptor.CreateInvalid(ex);
            }
        }

        public static StepDescriptor ToSynchronousStep(Action step)
        {
            try
            {
                var methodInfo = step.GetMethodInfo();
                EnsureNotGenerated(methodInfo);
                return new StepDescriptor(methodInfo, new StepExecutor(step).Execute);
            }
            catch (Exception ex)
            {
                return StepDescriptor.CreateInvalid(ex);
            }
        }

        private static void EnsureNotGenerated(MethodInfo methodInfo)
        {
            if (Reflector.IsGenerated(methodInfo))
                throw new ArgumentException($"The basic step syntax does not support compiler generated methods, such as {methodInfo}, as rendered step name will be unreadable. Please either pass the step method name directly or use other methods for declaring steps.");
        }

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