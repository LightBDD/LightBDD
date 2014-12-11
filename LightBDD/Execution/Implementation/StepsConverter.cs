using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LightBDD.Execution.Implementation.Parameters;
using LightBDD.Results;

namespace LightBDD.Execution.Implementation
{
    [DebuggerStepThrough]
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

        public IEnumerable<IStep> Convert(IEnumerable<Expression<Action<StepType>>> steps)
        {
            int i = 0;
            return steps.Select(step => CreateStep(step, ++i));
        }

        public IEnumerable<IStep> Convert<TContext>(TContext context, IEnumerable<Expression<Action<StepType, TContext>>> steps)
        {
            int i = 0;
            return steps.Select(step => CreateStep(context, step, ++i));
        }

        private IStep CreateStep(Expression<Action<StepType>> stepExpression, int stepNumber)
        {
            var stepTypeParameter = stepExpression.Parameters[0];
            var contextParameter = Expression.Parameter(typeof(object));
            return CreateStep((object)null, stepExpression, stepNumber, stepTypeParameter, contextParameter);
        }

        private IStep CreateStep<TContext>(TContext context, Expression<Action<StepType, TContext>> stepExpression, int stepNumber)
        {
            var stepTypeParameter = stepExpression.Parameters[0];
            var contextParameter = stepExpression.Parameters[1];
            return CreateStep(context, stepExpression, stepNumber, stepTypeParameter, contextParameter);
        }

        private IStep CreateStep<TExpression, TContext>(TContext context, Expression<TExpression> stepExpression, int stepNumber, ParameterExpression stepTypeParameter, ParameterExpression contextParameter)
        {
            var methodExpression = GetMethodExpression(stepExpression);
            var stepNameFormat = _metadataProvider.GetStepNameFormat(methodExpression.Method);
            var stepTypeName = _metadataProvider.GetStepTypeName(stepTypeParameter.Name);

            var action = CompileAction<TContext>(methodExpression, stepTypeParameter, contextParameter);
            var methodParameterInfo = methodExpression.Method.GetParameters();
            var arguments = methodExpression.Arguments.Select((arg, index) => CompileArgument<TContext>(arg, stepTypeParameter, contextParameter, methodParameterInfo[index])).ToArray();
            return new ParameterizedStep<TContext>(methodExpression.Method, context, action, arguments, stepTypeName, stepNameFormat, stepNumber, _mapExceptionToStatus);
        }

        private IStepParameter<TContext> CompileArgument<TContext>(Expression argumentExpression, ParameterExpression stepTypeParameter, ParameterExpression contextParameter, ParameterInfo parameterInfo)
        {
            var formatter = _metadataProvider.GetStepParameterFormatter(parameterInfo);
            var expression = argumentExpression as ConstantExpression;
            if (expression != null)
                return new ConstantStepParameter<TContext>(parameterInfo.Name, expression.Value, formatter);

            var compiledParam = Expression.Lambda<Func<StepType, TContext, object>>(Expression.Convert(argumentExpression, typeof(object)), stepTypeParameter, contextParameter).Compile();
            return new MutableStepParameter<TContext>(parameterInfo.Name, compiledParam, formatter);
        }

        private static Action<StepType, TContext, object[]> CompileAction<TContext>(MethodCallExpression methodCall, ParameterExpression stepTypeParameter, ParameterExpression contextParameter)
        {
            var param = Expression.Parameter(typeof(object[]), "args");
            var args = methodCall.Arguments.Select((arg, index) => Expression.Convert(Expression.ArrayAccess(param, Expression.Constant(index)), arg.Type));
            var methodCallExpression = Expression.Call(methodCall.Object, methodCall.Method, args);
            return Expression.Lambda<Action<StepType, TContext, object[]>>(methodCallExpression, stepTypeParameter, contextParameter, param).Compile();
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