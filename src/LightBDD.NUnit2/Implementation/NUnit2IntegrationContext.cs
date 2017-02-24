using System;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Framework.Formatting.Configuration;
using LightBDD.Framework.Notification.Configuration;
using NUnit.Framework;

namespace LightBDD.NUnit2.Implementation
{
    [DebuggerStepThrough]
    internal class NUnit2IntegrationContext : IIntegrationContext
    {
        public INameFormatter NameFormatter { get; }
        public IMetadataProvider MetadataProvider { get; }
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        public IExecutionExtensions ExecutionExtensions { get; }

        public NUnit2IntegrationContext(LightBddConfiguration configuration)
        {
            NameFormatter = configuration.NameFormatterConfiguration().Formatter;
            MetadataProvider = new NUnit2MetadataProvider(NameFormatter, configuration.StepTypeConfiguration(), configuration.CultureInfoProviderConfiguration().CultureInfoProvider);
            ExceptionToStatusMapper = MapExceptionToStatus;
            FeatureProgressNotifier = configuration.FeatureProgressNotifierConfiguration().Notifier;
            ScenarioProgressNotifierProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;
            ExecutionExtensions = configuration.ExecutionExtensionsConfiguration();
        }

        private static ExecutionStatus MapExceptionToStatus(Exception ex)
        {
            return (ex is IgnoreException || ex is InconclusiveException) 
                ? ExecutionStatus.Ignored 
                : ExecutionStatus.Failed;
        }
    }
}