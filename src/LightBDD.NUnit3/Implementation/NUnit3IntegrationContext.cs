using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    internal class NUnit3IntegrationContext : DefaultIntegrationContext
    {
        public NUnit3IntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration), MapExceptionToStatus)
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new NUnit3MetadataProvider(configuration);
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException || ex is InconclusiveException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}