using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Results;
using LightBDD.Framework.Extensibility;

namespace LightBDD.UnitTests.Helpers.TestableIntegration
{
    internal class TestableIntegrationContext : IntegrationContext
    {
        private readonly Func<Exception, ExecutionStatus> _exceptionMapper;

        public TestableIntegrationContext(LightBddConfiguration configuration, IMetadataProvider metadataProvider, Func<Exception, ExecutionStatus> exceptionMapper)
            : base(configuration, metadataProvider)
        {
            _exceptionMapper = exceptionMapper;
        }

        protected override ExecutionStatus MapExceptionToStatus(Exception exception) => _exceptionMapper(exception);
    }
}