using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Formatting.Configuration;
using LightBDD.Integration.XUnit2.Customization;
using LightBDD.Notification.Configuration;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnit2IntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        public IExecutionExtensions ExecutionExtensions { get; }

        public XUnit2IntegrationContext(LightBddConfiguration configuration)
        {
            NameFormatter = configuration.NameFormatterConfiguration().Formatter;
            MetadataProvider = new XUnit2MetadataProvider(NameFormatter, configuration.StepTypeConfiguration(), configuration.CultureInfoProviderConfiguration().CultureInfoProvider);
            ExceptionToStatusMapper = ex => (ex is IgnoreException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            FeatureProgressNotifier = configuration.FeatureProgressNotifierConfiguration().Notifier;
            ScenarioProgressNotifierProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;
            ExecutionExtensions = configuration.ExecutionExtensionsConfiguration();
        }
    }
}