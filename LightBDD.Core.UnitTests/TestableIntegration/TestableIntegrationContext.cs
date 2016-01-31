using System;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableIntegrationContext : IIntegrationContext
    {
        public TestableIntegrationContext()
        {
            MetadataProvider = new TestMetadataProvider();
            var defaultNameFormatter = new DefaultNameFormatter();
            NameFormatter = defaultNameFormatter;
            ExceptionToStatusMapper = ex => (ex is CustomIgnoreException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
        }
        public IMetadataProvider MetadataProvider { get; private set; }
        public INameFormatter NameFormatter { get; private set; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; private set; }
    }
}