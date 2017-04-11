using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using LightBDD.Framework.Formatting.Configuration;
using LightBDD.Framework.Notification.Configuration;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// A default implementation of <see cref="IIntegrationContext"/> and <see cref="IConfigurationContext"/>.
    /// </summary>
    public abstract class IntegrationContext : IIntegrationContext, IConfigurationContext
    {
        /// <summary>
        /// Returns metadata provider.
        /// </summary>
        public IMetadataProvider MetadataProvider { get; }

        /// <summary>
        /// Returns name formatter.
        /// </summary>
        public INameFormatter NameFormatter { get; }

        /// <summary>
        /// Returns exception to status mapping method.
        /// </summary>
        public Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }

        /// <summary>
        /// Returns feature progress notifier.
        /// </summary>
        public IFeatureProgressNotifier FeatureProgressNotifier { get; }

        /// <summary>
        /// Returns scenario progress notifier provider method.
        /// </summary>
        public Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }

        /// <summary>
        /// Returns LightBDD execution extensions.
        /// </summary>
        public IExecutionExtensions ExecutionExtensions { get; }

        /// <summary>
        /// Returns LightBDD Configuration.
        /// </summary>
        public LightBddConfiguration Configuration { get; }

        /// <summary>
        /// Default constructor sealing provided <paramref name="configuration"/> and initializing all properties.
        /// </summary>
        /// <param name="configuration">Configuration to use.</param>
        /// <param name="metadataProvider"><see cref="IMetadataProvider"/> instance to use.</param>
        protected IntegrationContext(LightBddConfiguration configuration, IMetadataProvider metadataProvider)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (metadataProvider == null)
                throw new ArgumentNullException(nameof(metadataProvider));

            Configuration = configuration.Seal();
            NameFormatter = configuration.NameFormatterConfiguration().Formatter;
            MetadataProvider = metadataProvider;
            ExceptionToStatusMapper = MapExceptionToStatus;
            FeatureProgressNotifier = configuration.FeatureProgressNotifierConfiguration().Notifier;
            ScenarioProgressNotifierProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;
            ExecutionExtensions = configuration.ExecutionExtensionsConfiguration();
        }

        /// <summary>
        /// Maps exception instance referenced by <paramref name="exception"/> to <see cref="ExecutionStatus"/>.
        /// </summary>
        /// <param name="exception">Exception to map.</param>
        /// <returns>Mapped status.</returns>
        protected abstract ExecutionStatus MapExceptionToStatus(Exception exception);
    }
}