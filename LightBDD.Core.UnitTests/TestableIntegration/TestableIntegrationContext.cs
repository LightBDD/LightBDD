using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableIntegrationContext : IIntegrationContext
    {
        public TestableIntegrationContext(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifier)
            : this(featureProgressNotifier, scenarioProgressNotifier, new ExecutionExtensionsConfiguration().EnableStepCommenting()) { }

        public TestableIntegrationContext(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifier, IExecutionExtensions executionExtensions)
        {
            NameFormatter = new DefaultNameFormatter();
            MetadataProvider = new TestMetadataProvider(NameFormatter);
            ExceptionToStatusMapper = ex => ex is CustomIgnoreException ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            FeatureProgressNotifier = featureProgressNotifier;
            ScenarioProgressNotifierProvider = scenarioProgressNotifier;
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