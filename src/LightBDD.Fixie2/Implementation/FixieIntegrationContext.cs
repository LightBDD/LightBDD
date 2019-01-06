using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Fixie2.Implementation
{
    internal class FixieIntegrationContext : DefaultIntegrationContext
    {
        public FixieIntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration), MapExceptionToStatus)
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new FixieMetadataProvider(configuration);
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}