using System;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    static class TestSyntax
    {
        public static void TestScenario(this IBddRunner runner, params Action[] steps)
        {
            runner.Integrate()
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(ToStepDescriptor))
                .RunAsync()
                .GetAwaiter()
                .GetResult();
        }

        public static Tuple<Action<object>, object> ParameterizedWithConstant(Action<object> action, object argument)
        {
            return new Tuple<Action<object>, object>(action, argument);
        }

        public static Tuple<Action<object>, Func<object, object>> ParameterizedWithFunction(Action<object> action, Func<object> argumentFunction)
        {
            return new Tuple<Action<object>, Func<object, object>>(action, obj => argumentFunction());
        }

        public static void TestParameterizedScenario(this IBddRunner runner, params Tuple<Action<object>, object>[] steps)
        {
            runner.Integrate()
                 .NewScenario()
                 .WithCapturedScenarioDetails()
                 .WithSteps(steps.Select(step => ToStepDescriptor(runner.Integrate(), step)))
                 .RunAsync()
                 .GetAwaiter()
                 .GetResult();
        }

        public static void TestParameterizedScenario(this IBddRunner runner, params Tuple<Action<object>, Func<object, object>>[] steps)
        {
            runner.Integrate()
                 .NewScenario()
                 .WithCapturedScenarioDetails()
                 .WithSteps(steps.Select(step => ToStepDescriptor(runner.Integrate(), step)))
                 .RunAsync()
                 .GetAwaiter()
                 .GetResult();
        }

        public static void TestNamedParameterizedScenario(this IBddRunner runner, string name, params Tuple<Action<object>, object>[] steps)
        {
            runner.Integrate()
                 .NewScenario()
                 .WithName(name)
                 .WithSteps(steps.Select(step => ToStepDescriptor(runner.Integrate(), step)))
                 .RunAsync()
                 .GetAwaiter()
                 .GetResult();
        }

        private static StepDescriptor ToStepDescriptor(ICoreBddRunner runner, Tuple<Action<object>, object> step)
        {
            var methodInfo = step.Item1.Method;
            var parameterInfo = methodInfo.GetParameters()[0];
            Func<object, object[], Task> stepInvocation = async (ctx, args) =>
            {
                await Task.Yield();
                step.Item1(args[0]);
            };

            return new StepDescriptor(
                methodInfo.Name,
                stepInvocation,
                runner.IntegrationContext.ParameterFactory.FromConstant(parameterInfo, step.Item2));
        }

        private static StepDescriptor ToStepDescriptor(ICoreBddRunner runner, Tuple<Action<object>, Func<object, object>> step)
        {
            var methodInfo = step.Item1.Method;
            var parameterInfo = methodInfo.GetParameters()[0];
            Func<object, object[], Task> stepInvocation = async (ctx, args) =>
            {
                await Task.Yield();
                step.Item1(args[0]);
            };

            return new StepDescriptor(
                methodInfo.Name,
                stepInvocation,
                runner.IntegrationContext.ParameterFactory.FromInvocation(parameterInfo, step.Item2));
        }

        private static StepDescriptor ToStepDescriptor(Action action)
        {
            return new StepDescriptor(action.Method.Name, (ctx, args) =>
            {
                action();
                return Task.FromResult(0);
            });
        }
    }
}