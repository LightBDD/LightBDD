using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Formatting.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.MsTest2.Implementation
{
    [DebuggerStepThrough]
    internal class MsTest2IntegrationContext : IntegrationContext
    {
        public MsTest2IntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration))
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new MsTest2MetadataProvider(
                configuration.NameFormatterConfiguration().Formatter,
                configuration.StepTypeConfiguration(),
                configuration.CultureInfoProviderConfiguration().CultureInfoProvider);
        }

        protected override ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return (ex is AssertInconclusiveException)
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}