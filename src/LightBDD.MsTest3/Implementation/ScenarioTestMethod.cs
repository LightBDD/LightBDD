using System;
using System.Reflection;
using System.Threading.Tasks;
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

        public async Task<TestResult> InvokeAsync(object[] arguments)
        {
            arguments ??= Arguments;
            try
            {
                TestContextProvider.Initialize(_target.MethodInfo, arguments);
                return await _target.InvokeAsync(arguments).ConfigureAwait(false);
            }
            finally
            {
                TestContextProvider.Clear();
            }
        }

        public Attribute[] GetAllAttributes()
        {
            return _target.GetAllAttributes();
        }

        public TAttributeType[] GetAttributes<TAttributeType>() where TAttributeType : Attribute
        {
            return _target.GetAttributes<TAttributeType>();
        }

        public string TestMethodName => _target.TestMethodName;
        public string TestClassName => _target.TestClassName;
        public Type ReturnType => _target.ReturnType;
        public object[] Arguments => _target.Arguments;
        public ParameterInfo[] ParameterTypes => _target.ParameterTypes;
        public MethodInfo MethodInfo => _target.MethodInfo;
    }
}