using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using System;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification;
using LightBDD.Framework.Notification.Implementation;

namespace LightBDD.Framework.Extensibility
{
    /// <summary>
    /// A default implementation of <see cref="IntegrationContext"/>.
    /// </summary>
    public class DefaultIntegrationContext : IntegrationContext
    {
        /// <inheritdoc />
        public override CoreMetadataProvider MetadataProvider { get; }

        /// <inheritdoc />
        public override Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }

        /// <inheritdoc />
        public override IFeatureProgressNotifier FeatureProgressNotifier { get; }

        /// <inheritdoc />
        public override Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }

        /// <inheritdoc />
        public override IExecutionExtensions ExecutionExtensions { get; }

        /// <inheritdoc />
        public override LightBddConfiguration Configuration { get; }

        /// <inheritdoc />
        public override IDependencyContainer DependencyContainer { get; }

        /// <inheritdoc />
        public override INameFormatter NameFormatter => MetadataProvider.NameFormatter;

        /// <inheritdoc />
        public override ValueFormattingService ValueFormattingService => MetadataProvider.ValueFormattingService;

        /// <summary>
        /// Default constructor sealing provided <paramref name="configuration"/> and initializing all properties.
        /// </summary>
        /// <param name="configuration">Configuration to use.</param>
        /// <param name="metadataProvider"><see cref="CoreMetadataProvider"/> instance to use.</param>
        /// <param name="exceptionToStatusMapper">Exception to status mapper function.</param>
        public DefaultIntegrationContext(LightBddConfiguration configuration, CoreMetadataProvider metadataProvider, Func<Exception, ExecutionStatus> exceptionToStatusMapper)
        {
            Configuration = configuration?.Seal() ?? throw new ArgumentNullException(nameof(configuration));
            MetadataProvider = metadataProvider ?? throw new ArgumentNullException(nameof(metadataProvider));

            ExceptionToStatusMapper = exceptionToStatusMapper;
            FeatureProgressNotifier = configuration.FeatureProgressNotifierConfiguration().Notifier;
            ScenarioProgressNotifierProvider = configuration.ScenarioProgressNotifierConfiguration().NotifierProvider;
            ExecutionExtensions = configuration.ExecutionExtensionsConfiguration();
            DependencyContainer = configuration.DependencyContainerConfiguration().DependencyContainer;
        }

        /// <inheritdoc />
        protected override IProgressNotifier GetProgressNotifier()
        {
            var notifier = Configuration.ProgressNotifierConfiguration().Notifier;

            if (Configuration.ScenarioProgressNotifierConfiguration().HasAny ||
                FeatureProgressNotifier != NoProgressNotifier.Default)
                notifier = DelegatingProgressNotifier.Compose(notifier, new NotificationAdapter(FeatureProgressNotifier, ScenarioProgressNotifierProvider));

            return notifier;
        }
    }
}