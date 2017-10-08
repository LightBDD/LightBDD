using System;
using System.Reflection;

namespace LightBDD.Core.UnitTests.Helpers
{
    public static class ParameterInfoHelper
    {
        private static void SomeMethod(int param) { }
        public static readonly ParameterInfo IntParameterInfo = GetMethodParameter<int>(SomeMethod);
        public static ParameterInfo GetMethodParameter<T>(Action<T> lambda)
        {
            return lambda.GetMethodInfo().GetParameters()[0];
        }
    }
}