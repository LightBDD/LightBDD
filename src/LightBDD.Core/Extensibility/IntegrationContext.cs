using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;
using System;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// A context offering integration objects used to configure runners.
    /// In comparison to <see cref="IntegrationContext"/>, this class offers also <see cref="Configuration"/> property to retrieve configuration.
    /// </summary>
    public abstract class IntegrationContext
    {
        /// <summary>
        /// Returns metadata provider.
        /// </summary>
        public abstract IMetadataProvider MetadataProvider { get; }

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
        public abstract IFeatureProgressNotifier FeatureProgressNotifier { get; }

        /// <summary>
        /// Returns scenario progress notifier provider method.
        /// </summary>
        public abstract Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }

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
    }
}