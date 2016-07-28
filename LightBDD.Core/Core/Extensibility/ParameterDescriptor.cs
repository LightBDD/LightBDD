using System;
using System.Reflection;

namespace LightBDD.Core.Extensibility
{
    public class ParameterDescriptor
    {
        private ParameterDescriptor(bool isConstant, ParameterInfo parameterInfo, Func<object, object> valueEvaluator)
        {
            if (parameterInfo == null)
                throw new ArgumentNullException(nameof(parameterInfo));
            if (valueEvaluator == null)
                throw new ArgumentNullException(nameof(valueEvaluator));
            RawName = parameterInfo.Name;
            IsConstant = isConstant;
            ParameterInfo = parameterInfo;
            ValueEvaluator = valueEvaluator;
        }

        public static ParameterDescriptor FromConstant(ParameterInfo parameterInfo, object value) => new ParameterDescriptor(true, parameterInfo, ctx => value);
        public static ParameterDescriptor FromInvocation(ParameterInfo parameterInfo, Func<object, object> valueEvaluator) => new ParameterDescriptor(false, parameterInfo, valueEvaluator);

        public string RawName { get; private set; }
        public bool IsConstant { get; private set; }
        public ParameterInfo ParameterInfo { get; private set; }
        public Func<object, object> ValueEvaluator { get; private set; }
    }
}