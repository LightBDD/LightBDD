using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Integration.XUnit2.Customization;

namespace LightBDD.Integration.XUnit2
{
    internal class XUnit2IntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }

        public XUnit2IntegrationContext(LightBddConfiguration configuration)
        {
            NameFormatter = configuration.Get<NameFormatterConfiguration>().Formatter;
            MetadataProvider = new XUnit2MetadataProvider(NameFormatter, configuration.Get<StepTypeConfiguration>());
            ExceptionToStatusMapper = ex => (ex is IgnoreException) ? ExecutionStatus.Ignored : ExecutionStatus.Failed;
            FeatureProgressNotifier = configuration.Get<FeatureProgressNotifierConfiguration>().Notifier;
            ScenarioProgressNotifierProvider = configuration.Get<ScenarioProgressNotifierConfiguration>().NotifierProvider;
        }
    }
}