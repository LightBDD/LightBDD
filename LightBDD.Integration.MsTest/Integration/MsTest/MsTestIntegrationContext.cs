using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LightBDD.Integration.MsTest
{
    internal class MsTestIntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        public IExecutionExtensions ExecutionExtensions { get; }

        public MsTestIntegrationContext(LightBddConfiguration configuration)
        {
            NameFormatter = configuration.NameFormatterConfiguration().Formatter;
            MetadataProvider = new MsTestMetadataProvider(NameFormatter, configuration.StepTypeConfiguration(), configuration.CultureInfoProviderConfiguration().CultureInfoProvider);
            ExceptionToStatusMapper = ex => (ex is AssertInconclusiveException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            FeatureProgressNotifier = configuration.FeatureProgressNotifierConfiguration().Notifier;
            ScenarioProgressNotifierProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;
            ExecutionExtensions = configuration.ExecutionExtensionsConfiguration();
        }
    }
}