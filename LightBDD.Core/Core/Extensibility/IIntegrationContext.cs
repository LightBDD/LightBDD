using System;
using LightBDD.Core.Formatting;

namespace LightBDD.Core.Extensibility
{
    public interface IIntegrationContext
    {
        IMetadataProvider MetadataProvider { get; }
        INameFormatter NameFormatter { get; }
        Func<Exception,ExecutionStatus> ExceptionToStatusMapper { get; }
        IParameterDescriptorFactory ParameterFactory { get; }
    }
}