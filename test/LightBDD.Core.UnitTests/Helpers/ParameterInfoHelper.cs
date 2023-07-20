using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LightBDD.Core.UnitTests.Helpers
{
    public static class ParameterInfoHelper
    {
        private static void SomeMethod(int param) { }
        public static readonly ParameterInfo IntParameterInfo = GetMethodParameter<int>(SomeMethod);
        public static ParameterInfo GetMethodParameter<T>(Action<T> lambda)
        {
            return GetMethodInfo(lambda).GetParameters()[0];
        }

        public static MethodInfo GetMethodInfo<T>(Action<T> lambda)
        {
            return lambda.GetMethodInfo();
        }

        public static MethodInfo GetMethodInfo(Action lambda)
        {
            return lambda.GetMethodInfo();
        }

        public static MethodInfo GetMethodInfo<T>(Expression<T> methodSelector)
        {
            if (methodSelector.Body is MethodCallExpression callExpression)
                return callExpression.Method;
            throw new InvalidOperationException($"Expected {nameof(methodSelector)} to be method call expression, got: {methodSelector}");
        }
    }
}