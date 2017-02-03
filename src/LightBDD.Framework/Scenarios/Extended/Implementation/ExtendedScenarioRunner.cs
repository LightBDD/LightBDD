using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Extended.Implementation
{
    [DebuggerStepThrough]
    internal class ExtendedScenarioRunner<TContext>
    {
        private readonly IBddRunner<TContext> _runner;

        public ExtendedScenarioRunner(IBddRunner<TContext> runner)
        {
            _runner = runner;
        }

        public void RunScenario(params Expression<Action<TContext>>[] steps)
        {
            _runner.Integrate()
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(ToStep))
                .RunSynchronously();
        }

        public Task RunScenarioAsync(params Expression<Func<TContext, Task>>[] steps)
        {
            return _runner.Integrate()
                .NewScenario()
                .WithCapturedScenarioDetails()
                .WithSteps(steps.Select(ToStep))
                .RunAsynchronously();
        }

        private StepDescriptor ToStep<T>(Expression<T> stepExpression)
        {
            var contextParameter = stepExpression.Parameters[0];
            var methodExpression = GetMethodExpression(stepExpression);

            var arguments = ProcessArguments(methodExpression, contextParameter);

            return new StepDescriptor(GetStepTypeName(contextParameter), methodExpression.Method.Name, CompileStepAction(methodExpression, contextParameter), arguments);
        }

        private static string GetStepTypeName(ParameterExpression contextParameter)
        {
            return contextParameter.Name == "_"
                ? null
                : contextParameter.Name;
        }

        private ParameterDescriptor[] ProcessArguments(MethodCallExpression methodExpression, ParameterExpression contextParameter)
        {
            var methodParameterInfo = methodExpression.Method.GetParameters();
            return methodExpression.Arguments.Select((arg, index) => CompileArgument(arg, contextParameter, methodParameterInfo[index])).ToArray();
        }

        private Func<object, object[], Task> CompileStepAction(MethodCallExpression methodCall, ParameterExpression contextParameter)
        {
            Func<int, Task> fromResult = Task.FromResult;

            var param = Expression.Parameter(typeof(object[]), "args");
            var args = methodCall.Arguments.Select((arg, index) => Expression.Convert(Expression.ArrayAccess(param, Expression.Constant(index)), arg.Type));
            Expression body = Expression.Call(methodCall.Object, methodCall.Method, args);
            if (methodCall.Method.ReturnType != typeof(Task))
                body = Expression.Block(body, Expression.Call(null, fromResult.GetMethodInfo(), Expression.Constant(0)));
            var function = Expression.Lambda<Func<TContext, object[], Task>>(body, contextParameter, param).Compile();

            return (o, a) => function((TContext)o, a);
        }

        private ParameterDescriptor CompileArgument(Expression argumentExpression, ParameterExpression contextParameter, ParameterInfo parameterInfo)
        {
            var expression = argumentExpression as ConstantExpression;
            if (expression != null)
                return ParameterDescriptor.FromConstant(parameterInfo, expression.Value);

            var function = Expression.Lambda<Func<TContext, object>>(Expression.Convert(argumentExpression, typeof(object)), contextParameter).Compile();

            return ParameterDescriptor.FromInvocation(parameterInfo, ctx => function((TContext)ctx));
        }

        private static MethodCallExpression GetMethodExpression<T>(Expression<T> stepExpression)
        {
            var methodExpression = stepExpression.Body as MethodCallExpression;
            if (methodExpression == null)
                throw new ArgumentException("Unsupported step expression. Expected MethodCallExpression, got: " + stepExpression);

            if (methodExpression.Method.GetParameters().Any(p => p.IsOut || p.ParameterType.IsByRef))
                throw new ArgumentException("Steps accepting ref or out parameters are not supported: " + stepExpression);
            return methodExpression;
        }
    }
}