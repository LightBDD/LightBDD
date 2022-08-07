using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using System;
using LightBDD.Core.Execution;
using LightBDD.Core.Execution.Implementation;
using LightBDD.Core.Formatting.Values;
using LightBDD.Core.Notification.Implementation;
using LightBDD.Core.Reporting;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// A context offering integration objects used to configure runners.
    /// </summary>
    //TODO: LightBDD 4.x Change test framework integration to use LightBDD Configuration instead of IntegrationContext. Move IntegrationContext to be internal in Core
    public abstract class IntegrationContext
    {
        private IProgressNotifier _progressNotifier;

        /// <summary>
        /// Returns metadata provider.
        /// </summary>
        public abstract CoreMetadataProvider MetadataProvider { get; }

        /// <summary>
        /// Returns name formatter.
        /// </summary>
        public abstract INameFormatter NameFormatter { get; }

        /// <summary>
        /// Returns exception to status mapping method.
        /// </summary>
        public abstract Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }

        /// <summary>
        /// Returns feature progress notifier.
        /// </summary>
        [Obsolete]
        public abstract IFeatureProgressNotifier FeatureProgressNotifier { get; }

        /// <summary>
        /// Returns scenario progress notifier provider method.
        /// </summary>
        [Obsolete]
        public abstract Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }

        /// <summary>
        /// Returns progress notifier.
        /// </summary>
        public IProgressNotifier ProgressNotifier => _progressNotifier ??= GetProgressNotifier();

        /// <summary>
        /// Returns LightBDD execution extensions.
        /// </summary>
        public abstract IExecutionExtensions ExecutionExtensions { get; }

        /// <summary>
        /// Returns <see cref="LightBddConfiguration"/> used to configure tests.
        /// It is expected that returned object will be sealed which means that it should be used only for reading configuration, but not altering it.
        /// </summary>
        public abstract LightBddConfiguration Configuration { get; }

        /// <summary>
        /// Returns configured <see cref="IDependencyContainer"/>, used to resolve dependencies for LightBDD contexts.
        /// </summary>
        public abstract IDependencyContainer DependencyContainer { get; }

        /// <summary>
        /// Returns value formatting service.
        /// </summary>
        public abstract ValueFormattingService ValueFormattingService { get; }

        /// <summary>
        /// Returns execution timer.
        /// </summary>
        public IExecutionTimer ExecutionTimer { get; } = DefaultExecutionTimer.StartNew();

        /// <summary>
        /// Returns File Attachments Manager
        /// </summary>
        public virtual IFileAttachmentsManager FileAttachmentsManager => Configuration.ReportWritersConfiguration().GetFileAttachmentsManager();

        /// <summary>
        /// Creates progress notifier.
        /// </summary>
#pragma warning disable 612
        protected virtual IProgressNotifier GetProgressNotifier() => new NotificationAdapter(FeatureProgressNotifier, ScenarioProgressNotifierProvider);
#pragma warning restore 612
    }
}