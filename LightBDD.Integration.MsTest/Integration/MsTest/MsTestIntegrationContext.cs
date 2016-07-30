using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest
{
    internal class MsTestIntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IProgressNotifier ProgressNotifier { get; }

        public MsTestIntegrationContext(LightBddConfiguration configuration, IProgressNotifier progressNotifier)
        {
            NameFormatter = configuration.Get<NameFormatterConfiguration>().Formatter;
            MetadataProvider = new MsTestMetadataProvider(NameFormatter, configuration.Get<StepTypeConfiguration>());
            ExceptionToStatusMapper = ex => (ex is AssertInconclusiveException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            ProgressNotifier = progressNotifier;
        }
    }
}