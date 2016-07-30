using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Integration.XUnit2.Customization;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnit2IntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IProgressNotifier ProgressNotifier { get; }

        public XUnit2IntegrationContext(LightBddConfiguration configuration, IProgressNotifier progressNotifier)
        {
            NameFormatter = configuration.Get<NameFormatterConfiguration>().Formatter;
            MetadataProvider = new XUnit2MetadataProvider(NameFormatter, configuration.Get<StepTypeConfiguration>());
            ExceptionToStatusMapper = ex => (ex is IgnoreException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            ProgressNotifier = progressNotifier;
        }
    }
}