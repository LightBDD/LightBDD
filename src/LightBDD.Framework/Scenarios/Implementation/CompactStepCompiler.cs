using System;
using System.Linq;
using System.Reflection;
using System.Text;
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
            try
            {
                return new StepDescriptor(ParseName(name), new AsyncStepExecutor<TContext>(step).ExecuteAsync);
            }
            catch (Exception ex)
            {
                return StepDescriptor.CreateInvalid(ex);
            }
        }

        public static StepDescriptor ToSynchronousStep<TContext>(string name, Action<TContext> step)
        {
            try
            {
                return new StepDescriptor(ParseName(name), new StepExecutor<TContext>(step).Execute);
            }
            catch (Exception ex)
            {
                return StepDescriptor.CreateInvalid(ex);
            }
        }

        private static string ParseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Step name has to be provided.");

            name = name.Trim();
            if (!name.Any(char.IsControl))
                return name;

            var builder = new StringBuilder(name.Length);
            var newLineChar = Environment.NewLine.Last();

            foreach (var c in name)
            {
                if (!char.IsControl(c))
                    builder.Append(c);
                else if (c == '\t' || c == newLineChar)
                    builder.Append(' ');
            }

            return builder.ToString();
        }

        private class AsyncStepExecutor<TContext>
        {
            private static readonly MethodInfo AsCompositeStepMethod = ((Func<Task, Task<IStepResultDescriptor>>)AsCompositeStep<IStepResultDescriptor>).GetMethodInfo().GetGenericMethodDefinition();
            private readonly Func<TContext, Task> _invocation;

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
                    _invocation.Invoke((TContext)context);
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