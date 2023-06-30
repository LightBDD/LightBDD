using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest3.Implementation
{
    internal class ScenarioTestMethod : ITestMethod
    {
        private readonly ITestMethod _target;

        public ScenarioTestMethod(ITestMethod target)
        {
            _target = target;
        }

        public TestResult Invoke(object[] arguments)
        {
            arguments ??= Arguments;
            try
            {
                TestContextProvider.Initialize(_target.MethodInfo, arguments);
                return _target.Invoke(arguments);
            }
            finally
            {
                TestContextProvider.Clear();
            }
        }

        public Attribute[] GetAllAttributes(bool inherit)
        {
            return _target.GetAllAttributes(inherit);
        }

        public TAttributeType[] GetAttributes<TAttributeType>(bool inherit) where TAttributeType : Attribute
        {
            return _target.GetAttributes<TAttributeType>(inherit);
        }

        public string TestMethodName => _target.TestMethodName;
        public string TestClassName => _target.TestClassName;
        public Type ReturnType => _target.ReturnType;
        public object[] Arguments => _target.Arguments;
        public ParameterInfo[] ParameterTypes => _target.ParameterTypes;
        public MethodInfo MethodInfo => _target.MethodInfo;
    }
}