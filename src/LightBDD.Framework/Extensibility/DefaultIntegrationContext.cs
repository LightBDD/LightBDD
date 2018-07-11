using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
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
    /// A default implementation of <see cref="IntegrationContext"/>.
    /// </summary>
    public class DefaultIntegrationContext : IntegrationContext
    {
        /// <summary>
        /// Returns metadata provider.
        /// </summary>
        public override IMetadataProvider MetadataProvider { get; }

        /// <summary>
        /// Returns name formatter.
        /// </summary>
        public override INameFormatter NameFormatter { get; }

        /// <summary>
        /// Returns exception to status mapping method.
        /// </summary>
        public override Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }

        /// <summary>
        /// Returns feature progress notifier.
        /// </summary>
        public override IFeatureProgressNotifier FeatureProgressNotifier { get; }

        /// <summary>
        /// Returns scenario progress notifier provider method.
        /// </summary>
        public override Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }

        /// <summary>
        /// Returns LightBDD execution extensions.
        /// </summary>
        public override IExecutionExtensions ExecutionExtensions { get; }

        /// <summary>
        /// Returns LightBDD Configuration.
        /// </summary>
        public override LightBddConfiguration Configuration { get; }

        /// <inheritdoc />
        public override IDependencyContainer DependencyContainer { get; }

        /// <summary>
        /// Default constructor sealing provided <paramref name="configuration"/> and initializing all properties.
        /// </summary>
        /// <param name="configuration">Configuration to use.</param>
        /// <param name="metadataProvider"><see cref="IMetadataProvider"/> instance to use.</param>
        /// <param name="exceptionToStatusMapper">Exception to status mapper function.</param>
        public DefaultIntegrationContext(LightBddConfiguration configuration, IMetadataProvider metadataProvider, Func<Exception, ExecutionStatus> exceptionToStatusMapper)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (metadataProvider == null)
                throw new ArgumentNullException(nameof(metadataProvider));

            Configuration = configuration.Seal();
            NameFormatter = configuration.NameFormatterConfiguration().Formatter;
            MetadataProvider = metadataProvider;
            ExceptionToStatusMapper = exceptionToStatusMapper;
            FeatureProgressNotifier = configuration.FeatureProgressNotifierConfiguration().Notifier;
            ScenarioProgressNotifierProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;
            ExecutionExtensions = configuration.ExecutionExtensionsConfiguration();
            DependencyContainer = configuration.DependencyContainerConfiguration().DependencyContainer;
        }
    }
}