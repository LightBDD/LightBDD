using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Integration.XUnit2.Customization;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnitIntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IProgressNotifier ProgressNotifier { get; }

        public XUnitIntegrationContext(IProgressNotifier progressNotifier)
        {
            NameFormatter = new DefaultNameFormatter();
            MetadataProvider = new XUnitMetadataProvider(NameFormatter);
            ExceptionToStatusMapper = ex => (ex is IgnoreException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            ProgressNotifier = progressNotifier;
        }
    }
}