using System;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    public static class TestStep
    {
        public static StepDescriptor CreateAsync(Action step)
        {
            return new StepDescriptor(step.GetMethodInfo(),
                async (ctx, args) =>
                {
                    await Task.Delay(10);
                    step.Invoke();
                    return DefaultStepResultDescriptor.Instance;
                });
        }

        public static StepDescriptor CreateSync(Action step)
        {
            return new StepDescriptor(step.GetMethodInfo(),
                (ctx, args) =>
                {
                    step.Invoke();
                    return Task.FromResult(DefaultStepResultDescriptor.Instance);
                });
        }

        public static StepDescriptor Create(Func<Task> step)
        {
            return new StepDescriptor(step.GetMethodInfo(), async (ctx, args) =>
            {
                await step.Invoke();
                return DefaultStepResultDescriptor.Instance;
            });
        }

        public static StepDescriptor CreateComposite(Func<TestCompositeStep> step)
        {
            return new StepDescriptor(
                step.GetMethodInfo(),
                (ctx, args) => Task.FromResult((IStepResultDescriptor)step.Invoke()));
        }

        public static StepDescriptor CreateAsync<TArg>(Action<TArg> step, Func<TArg> argEvaluator)
        {
            async Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                await Task.Yield();
                step.Invoke((TArg)args[0]);
                return DefaultStepResultDescriptor.Instance;
            }

            var parameter = ParameterDescriptor.FromInvocation(step.GetMethodInfo().GetParameters()[0], ctx => argEvaluator.Invoke());

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, parameter);
        }

        public static StepDescriptor CreateWithTypeAsync(string stepType, Action step)
        {
            return new StepDescriptor(
                step.GetMethodInfo(),
                async (ctx, args) =>
                {
                    await Task.Yield();
                    step.Invoke();
                    return DefaultStepResultDescriptor.Instance;
                })
            {
                PredefinedStepType = stepType
            };
        }

        public static StepDescriptor CreateSync<TArg>(Action<TArg> step, Func<TArg> argEvaluator)
        {
            Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                step.Invoke((TArg)args[0]);
                return Task.FromResult(DefaultStepResultDescriptor.Instance);
            }

            var parameter = ParameterDescriptor.FromInvocation(step.GetMethodInfo().GetParameters()[0], ctx => argEvaluator.Invoke());

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, parameter);
        }
        public static StepDescriptor Create<TArg>(Func<TArg, Task> step, Func<TArg> argEvaluator)
        {
            Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                step.Invoke((TArg)args[0]);
                return Task.FromResult(DefaultStepResultDescriptor.Instance);
            }

            var parameter = ParameterDescriptor.FromInvocation(step.GetMethodInfo().GetParameters()[0], ctx => argEvaluator.Invoke());

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, parameter);
        }

        public static StepDescriptor CreateAsync<TArg>(Action<TArg> step, TArg arg)
        {
            async Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                await Task.Yield();
                step.Invoke((TArg)args[0]);
                return DefaultStepResultDescriptor.Instance;
            }

            var parameter = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[0], arg);

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, parameter);
        }

        public static StepDescriptor CreateAsync<TArg1, TArg2>(Action<TArg1, TArg2> step, TArg1 arg1, TArg2 arg2)
        {
            async Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                await Task.Yield();
                step.Invoke((TArg1)args[0], (TArg2)args[1]);
                return DefaultStepResultDescriptor.Instance;
            }

            var p1 = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[0], arg1);
            var p2 = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[1], arg2);

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, p1, p2);
        }

        public static StepDescriptor CreateAsync<TArg1, TArg2, TArg3>(Action<TArg1, TArg2, TArg3> step, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            async Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                await Task.Yield();
                step.Invoke((TArg1)args[0], (TArg2)args[1], (TArg3)args[2]);
                return DefaultStepResultDescriptor.Instance;
            }

            var p1 = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[0], arg1);
            var p2 = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[1], arg2);
            var p3 = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[2], arg3);

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, p1, p2, p3);
        }

        public static StepDescriptor CreateSync<TArg>(Action<TArg> step, TArg arg)
        {
            Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                step.Invoke((TArg)args[0]);
                return Task.FromResult(DefaultStepResultDescriptor.Instance);
            }

            var parameter = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[0], arg);

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, parameter);
        }
        public static StepDescriptor Create<TArg>(Func<TArg, Task> step, TArg arg)
        {
            Task<IStepResultDescriptor> StepInvocation(object ctx, object[] args)
            {
                step.Invoke((TArg)args[0]);
                return Task.FromResult(DefaultStepResultDescriptor.Instance);
            }

            var parameter = ParameterDescriptor.FromConstant(step.GetMethodInfo().GetParameters()[0], arg);

            return new StepDescriptor(step.GetMethodInfo(), StepInvocation, parameter);
        }
    }
}