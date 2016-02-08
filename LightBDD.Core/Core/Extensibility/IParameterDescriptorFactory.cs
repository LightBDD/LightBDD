using System;
using System.Reflection;

namespace LightBDD.Core.Extensibility
{
    public interface IParameterDescriptorFactory
    {
        ParameterDescriptor FromConstant(ParameterInfo parameterInfo, object value);
        ParameterDescriptor FromInvocation(ParameterInfo parameterInfo, Func<object, object> valueEvaluator);
    }
}