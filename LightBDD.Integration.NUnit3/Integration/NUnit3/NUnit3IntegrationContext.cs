using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using NUnit.Framework;

namespace LightBDD.Integration.NUnit3
{
    internal class NUnit3IntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IProgressNotifier ProgressNotifier { get; }

        public NUnit3IntegrationContext(IProgressNotifier progressNotifier)
        {
            NameFormatter = new DefaultNameFormatter();
            MetadataProvider = new NUnit3MetadataProvider(NameFormatter);
            ExceptionToStatusMapper = ex => (ex is IgnoreException || ex is InconclusiveException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            ProgressNotifier = progressNotifier;
        }
    }
}