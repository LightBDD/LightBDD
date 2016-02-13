using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableIntegrationContext : IIntegrationContext
    {
        public TestableIntegrationContext()
        {
            NameFormatter = new DefaultNameFormatter();
            MetadataProvider = new TestMetadataProvider(NameFormatter);
            ExceptionToStatusMapper = ex => (ex is CustomIgnoreException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            ParameterFactory=new DefaultParameterDescriptorFactory(MetadataProvider);
        }
        public IMetadataProvider MetadataProvider { get; private set; }
        public INameFormatter NameFormatter { get; private set; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; private set; }
        public IParameterDescriptorFactory ParameterFactory { get; private set; }
    }
}