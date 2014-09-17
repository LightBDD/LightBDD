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

        public IEnumerable<Step> Convert(IEnumerable<Action> steps)
        {
            int i = 0;
            return steps.Select(step => new Step(step, _metadataProvider.GetStepName(step.Method), ++i, _mapExceptionToStatus));
        }

        public IEnumerable<Step> Convert<TContext>(TContext context, IEnumerable<Action<TContext>> steps)
        {
            int i = 0;
            return steps.Select(step => new Step(() => step(context), _metadataProvider.GetStepName(step.Method), ++i, _mapExceptionToStatus));
        }

        public IEnumerable<Step> Convert<TContext>(TContext context, IEnumerable<Expression<Action<TContext>>> steps)
        {
            int i = 0;
            return steps.Select(step => new Step(Compile(step, context), GetStepName(step), ++i, _mapExceptionToStatus));
        }

        private string GetStepName<T>(Expression<Action<T>> stepExpression)
        {
            var paramName = stepExpression.Parameters[0].Name;
            var methodExpression = (MethodCallExpression)stepExpression.Body;
            var arguments = methodExpression.Arguments.Select(GetArgumentValue).ToArray();
            return _metadataProvider.GetStepName(paramName, methodExpression.Method, arguments);
        }

        private object GetArgumentValue(Expression expression)
        {
            var constant = expression as ConstantExpression;
            if (constant != null)
                return constant.Value;
            throw new ArgumentException("Unsupported argument expression: " + expression.GetType());
        }

        private Action Compile<T>(Expression<Action<T>> step, T context)
        {
            var compiled = step.Compile();
            return () => compiled(context);
        }
    }
}