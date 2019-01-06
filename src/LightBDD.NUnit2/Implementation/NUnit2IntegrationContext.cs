using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using NUnit.Framework;

namespace LightBDD.NUnit2.Implementation
{
    internal class NUnit2IntegrationContext : DefaultIntegrationContext
    {
        public NUnit2IntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration), MapExceptionToStatus)
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new NUnit2MetadataProvider(configuration);
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is IgnoreException || ex is InconclusiveException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}