using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Extended.Implementation
{
    internal class ExtendedStepCompiler<TContext>
    {
        private readonly LightBddConfiguration _configuration;

        public ExtendedStepCompiler(LightBddConfiguration configuration)
        {
            _configuration = configuration;
        }

        public StepDescriptor ToStep<T>(Expression<T> stepExpression)
        {
            var contextParameter = stepExpression.Parameters[0];
            var methodExpression = GetMethodExpression(stepExpression);

            var arguments = ProcessArguments(methodExpression, contextParameter);

            return new StepDescriptor(GetStepTypeName(contextParameter), methodExpression.Method.Name, CompileStepAction(methodExpression, contextParameter), arguments);
        }

        private string GetStepTypeName(ParameterExpression contextParameter)
        {
            return _configuration.Get<StepTypeConfiguration>().UseLambdaNameAsStepType(contextParameter.Name)
                ? contextParameter.Name
                : null;
        }

        private ParameterDescriptor[] ProcessArguments(MethodCallExpression methodExpression, ParameterExpression contextParameter)
        {
            var methodParameterInfo = methodExpression.Method.GetParameters();
            return methodExpression.Arguments.Select((arg, index) => CompileArgument(arg, contextParameter, methodParameterInfo[index])).ToArray();
        }

        private Func<object, object[], Task<StepResultDescriptor>> CompileStepAction(MethodCallExpression methodCall, ParameterExpression contextParameter)
        {
            var targetParam = Expression.Parameter(typeof(object), "target");
            var argsParam = Expression.Parameter(typeof(object[]), "args");
            var args = methodCall.Arguments.Select((arg, index) => Expression.Convert(Expression.ArrayAccess(argsParam, Expression.Constant(index)), arg.Type));
            Expression body = Expression.Call(methodCall.Object, methodCall.Method, args);

            body = ConvertReturnType(methodCall.Method.ReturnType, body);

            var stepCallFunction = Expression.Lambda<Func<TContext, object[], Task<StepResultDescriptor>>>(body, contextParameter, argsParam).Compile();

            var genericStepCallFunction = Expression.Lambda<Func<object, object[], Task<StepResultDescriptor>>>(
                    Expression.Invoke(Expression.Constant(stepCallFunction), Expression.Convert(targetParam, contextParameter.Type), argsParam),
                    targetParam, argsParam)
                .Compile();
            return genericStepCallFunction;
        }

        private static Expression ConvertReturnType(Type currentReturnType, Expression body)
        {
            var currentTypeInfo = currentReturnType.GetTypeInfo();
            if (typeof(Task).GetTypeInfo().IsAssignableFrom(currentTypeInfo))
            {
                if (currentReturnType == typeof(Task<StepResultDescriptor>))
                    return body;
                if (currentTypeInfo.IsGenericType && typeof(StepResultDescriptor).GetTypeInfo()
                        .IsAssignableFrom(currentTypeInfo.GenericTypeArguments[0].GetTypeInfo()))
                {
                    Func<Task<StepResultDescriptor>, Task<StepResultDescriptor>> converter = ConvertTask;
                    var specializedConverter = converter.GetMethodInfo().GetGenericMethodDefinition().MakeGenericMethod(currentTypeInfo.GenericTypeArguments[0]);
                    return Expression.Call(null, specializedConverter, body);
                }
                Func<Task, Task<StepResultDescriptor>> finalizer = FinalizeTask;

                return Expression.Call(null, finalizer.GetMethodInfo(), body);
            }

            Func<StepResultDescriptor, Task> fromResult = Task.FromResult;
            if (typeof(StepResultDescriptor).GetTypeInfo().IsAssignableFrom(currentTypeInfo))
                return Expression.Call(null, fromResult.GetMethodInfo(), body);

            return Expression.Block(body, Expression.Call(null, fromResult.GetMethodInfo(), Expression.Constant(StepResultDescriptor.Default)));
        }

        private ParameterDescriptor CompileArgument(Expression argumentExpression, ParameterExpression contextParameter, ParameterInfo parameterInfo)
        {
            var expression = argumentExpression as ConstantExpression;
            if (expression != null)
                return ParameterDescriptor.FromConstant(parameterInfo, expression.Value);

            var function = Expression.Lambda<Func<TContext, object>>(Expression.Convert(argumentExpression, typeof(object)), contextParameter).Compile();

            var targetParam = Expression.Parameter(typeof(object), "target");
            var genericStepArgFunction = Expression.Lambda<Func<object, object>>(Expression.Invoke(Expression.Constant(function), Expression.Convert(targetParam, contextParameter.Type)), targetParam)
                .Compile();

            return ParameterDescriptor.FromInvocation(parameterInfo, genericStepArgFunction);
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

        private static async Task<StepResultDescriptor> ConvertTask<T>(Task<T> parent) where T : StepResultDescriptor
        {
            return await parent;
        }

        private static async Task<StepResultDescriptor> FinalizeTask(Task parent)
        {
            await parent;
            return StepResultDescriptor.Default;
        }
    }
}