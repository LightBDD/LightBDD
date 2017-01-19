using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    internal class TestableIntegrationContext : IIntegrationContext
    {
        public TestableIntegrationContext(INameFormatter nameFormatter, IMetadataProvider metadataProvider, Func<Exception, ExecutionStatus> exceptionToStatusMapper, IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifierProvider, IExecutionExtensions executionExtensions)
        {
            NameFormatter = nameFormatter;
            MetadataProvider = metadataProvider;
            ExceptionToStatusMapper = exceptionToStatusMapper;
            FeatureProgressNotifier = featureProgressNotifier;
            ScenarioProgressNotifierProvider = scenarioProgressNotifierProvider;
            ExecutionExtensions = executionExtensions;
        }

        public IMetadataProvider MetadataProvider { get; }
        public INameFormatter NameFormatter { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        public IExecutionExtensions ExecutionExtensions { get; }
    }
}