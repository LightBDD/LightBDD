using System;
using System.Reflection;

namespace LightBDD.Core.Extensibility
{
    public class ParameterDescriptor
    {
        private ParameterDescriptor(bool isConstant, ParameterInfo parameterInfo, Func<object, object> valueEvaluator)
        {
            RawName = parameterInfo.Name;
            IsConstant = isConstant;
            ParameterInfo = parameterInfo;
            ValueEvaluator = valueEvaluator;
        }

        public static ParameterDescriptor FromConstant(ParameterInfo parameter, object value) => new ParameterDescriptor(true, parameter, ctx => value);
        public static ParameterDescriptor FromInvocation(ParameterInfo parameter, Func<object, object> valueEvaluator) => new ParameterDescriptor(false, parameter, valueEvaluator);

        public string RawName { get; private set; }
        public bool IsConstant { get; private set; }
        public ParameterInfo ParameterInfo { get; private set; }
        public Func<object, object> ValueEvaluator { get; private set; }
    }
}