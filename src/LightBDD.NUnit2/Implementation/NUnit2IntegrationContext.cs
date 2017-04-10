using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Formatting.Configuration;
using NUnit.Framework;

namespace LightBDD.NUnit2.Implementation
{
    [DebuggerStepThrough]
    internal class NUnit2IntegrationContext : IntegrationContext
    {
        public NUnit2IntegrationContext(LightBddConfiguration configuration)
            : base(configuration, CreateMetadataProvider(configuration))
        {
        }

        private static IMetadataProvider CreateMetadataProvider(LightBddConfiguration configuration)
        {
            return new NUnit2MetadataProvider(
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