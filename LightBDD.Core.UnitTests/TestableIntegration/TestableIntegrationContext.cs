using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableIntegrationContext : IIntegrationContext
    {
        public TestableIntegrationContext(IProgressNotifier progressNotifier)
        {
            ProgressNotifier = progressNotifier;
            NameFormatter = new DefaultNameFormatter();
            MetadataProvider = new TestMetadataProvider(NameFormatter);
            ExceptionToStatusMapper = ex => (ex is CustomIgnoreException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            ParameterFactory = new DefaultParameterDescriptorFactory(MetadataProvider);
        }
        public IMetadataProvider MetadataProvider { get; }
        public INameFormatter NameFormatter { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IParameterDescriptorFactory ParameterFactory { get; }
        public IProgressNotifier ProgressNotifier { get; }
    }
}