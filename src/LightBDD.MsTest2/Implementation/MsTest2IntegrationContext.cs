using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2.Implementation
{
    internal class MsTest2IntegrationContext : DefaultIntegrationContext
    {
        public MsTest2IntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration), MapExceptionToStatus)
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new MsTest2MetadataProvider(configuration);
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return ex is AssertInconclusiveException
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}