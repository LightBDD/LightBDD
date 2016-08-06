using System;
using System.Collections.Generic;
using System.Linq;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Extensions.ContextualAsyncExecution;

namespace LightBDD.Core.UnitTests.TestableIntegration
{
    public class TestableIntegrationContext : IIntegrationContext
    {
        public TestableIntegrationContext(IFeatureProgressNotifier featureProgressNotifier, Func<object, IScenarioProgressNotifier> scenarioProgressNotifier)
        {
            NameFormatter = new DefaultNameFormatter();
            MetadataProvider = new TestMetadataProvider(NameFormatter);
            ExceptionToStatusMapper = ex => ex is CustomIgnoreException ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            FeatureProgressNotifier = featureProgressNotifier;
            ScenarioProgressNotifierProvider = scenarioProgressNotifier;
            ExecutionExtensions = new ExecutionExtensionsConfiguration().AddScenarioExtension(new ScenarioExecutionContextExtension()).AddStepExtension(new StepCommentingExtension());
        }

        public IMetadataProvider MetadataProvider { get; }
        public INameFormatter NameFormatter { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        public ExecutionExtensionsConfiguration ExecutionExtensions { get; }
    }
}