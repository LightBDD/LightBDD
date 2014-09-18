using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LightBDD.Results;

namespace LightBDD.Execution
{
    internal class StepsConverter : IStepsConverter
    {
        private readonly Func<Type, ResultStatus> _mapExceptionToStatus;
        private readonly TestMetadataProvider _metadataProvider;

        public StepsConverter(TestMetadataProvider metadataProvider, Func<Type, ResultStatus> mapExceptionToStatus)
        {
            _mapExceptionToStatus = mapExceptionToStatus;
            _metadataProvider = metadataProvider;
        }

        public IEnumerable<IStep> Convert(IEnumerable<Action> steps)
        {
            int i = 0;
            return steps.Select(step => new Step(step, _metadataProvider.GetStepName(step.Method), ++i, _mapExceptionToStatus));
        }

        public IEnumerable<IStep> Convert<TContext>(TContext context, IEnumerable<Action<TContext>> steps)
        {
            int i = 0;
            return steps.Select(step => new Step(() => step(context), _metadataProvider.GetStepName(step.Method), ++i, _mapExceptionToStatus));
        }

        public IEnumerable<IStep> Convert<TContext>(TContext context, IEnumerable<Expression<Action<TContext>>> steps)
        {
            int i = 0;
            return steps.Select(step => CreateStep(context, step, ++i));
        }

        private IStep CreateStep<TContext>(TContext context, Expression<Action<TContext>> stepExpression, int stepNumber)
        {
            var paramName = stepExpression.Parameters[0].Name;
            var methodExpression = stepExpression.Body as MethodCallExpression;
            if (methodExpression == null)
                throw new ArgumentException("Unsupported step expression. Expected MethodCallExpression, got: " + stepExpression);

            if (methodExpression.Method.GetParameters().Any(p => p.IsOut || p.ParameterType.IsByRef))
                throw new ArgumentException("Steps accepting ref or out parameters are not supported: " + stepExpression);
            var stepNameFormat = _metadataProvider.GetStepNameFormat(paramName, methodExpression.Method);

            var action = CompileAction<TContext>(methodExpression, stepExpression.Parameters[0]);
            var arguments = methodExpression.Arguments.Select(arg => CompileArgument<TContext>(arg, stepExpression.Parameters[0])).ToArray();
            return new ParameterizedStep<TContext>(context, action, arguments, stepNameFormat, stepNumber, _mapExceptionToStatus);
        }

        private Func<TContext, object> CompileArgument<TContext>(Expression argumentExpression, ParameterExpression contextParameter)
        {
            return Expression.Lambda<Func<TContext, object>>(Expression.Convert(argumentExpression, typeof(object)), contextParameter).Compile();
        }

        private static Action<TContext, object[]> CompileAction<TContext>(MethodCallExpression methodCall, ParameterExpression contextParameter)
        {
            var param = Expression.Parameter(typeof(object[]), "args");
            var args = methodCall.Arguments.Select((arg, index) => Expression.Convert(Expression.ArrayAccess(param, Expression.Constant(index)), arg.Type));
            var methodCallExpression = Expression.Call(methodCall.Object, methodCall.Method, args);
            return Expression.Lambda<Action<TContext, object[]>>(methodCallExpression, contextParameter, param).Compile();
        }
    }
}