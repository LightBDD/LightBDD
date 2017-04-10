using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Formatting.Configuration;
using NUnit.Framework;

namespace LightBDD.NUnit3.Implementation
{
    [DebuggerStepThrough]
    internal class NUnit3IntegrationContext : IntegrationContext
    {
        public NUnit3IntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration))
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new NUnit3MetadataProvider(
                configuration.NameFormatterConfiguration().Formatter,
                configuration.StepTypeConfiguration(),
                configuration.CultureInfoProviderConfiguration().CultureInfoProvider);
        }

        protected override ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return (ex is IgnoreException || ex is InconclusiveException)
                ? ExecutionStatus.Ignored
                : ExecutionStatus.Failed;
        }
    }
}