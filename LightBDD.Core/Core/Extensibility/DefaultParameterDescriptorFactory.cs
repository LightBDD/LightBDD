using System;
using System.Reflection;

namespace LightBDD.Core.Extensibility
{
    public class DefaultParameterDescriptorFactory : IParameterDescriptorFactory
    {
        private readonly IMetadataProvider _metadataProvider;

        public DefaultParameterDescriptorFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public ParameterDescriptor FromConstant(ParameterInfo parameterInfo, object value)
        {
            return new ParameterDescriptor(true, parameterInfo.Name, ctx => value, _metadataProvider.GetStepParameterFormatter(parameterInfo));
        }

        public ParameterDescriptor FromInvocation(ParameterInfo parameterInfo, Func<object, object> valueEvaluator)
        {
            return new ParameterDescriptor(false, parameterInfo.Name, valueEvaluator, _metadataProvider.GetStepParameterFormatter(parameterInfo));
        }
    }
}