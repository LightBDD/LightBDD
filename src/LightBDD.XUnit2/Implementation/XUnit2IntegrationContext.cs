using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.XUnit2.Implementation.Customization;

namespace LightBDD.XUnit2.Implementation
{
    [DebuggerStepThrough]
    internal class XUnit2IntegrationContext : DefaultIntegrationContext
    {
        public XUnit2IntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration), MapExceptionToStatus)
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new XUnit2MetadataProvider(configuration);
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}