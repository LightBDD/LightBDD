using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Interface describing integration context provided by projects integrating with test frameworks.
    /// The integration context is used to configure runners and used during scenario execution.
    /// </summary>
    public interface IIntegrationContext
    {
        /// <summary>
        /// Returns metadata provider.
        /// </summary>
        IMetadataProvider MetadataProvider { get; }
        /// <summary>
        /// Returns name formatter.
        /// </summary>
        INameFormatter NameFormatter { get; }
        /// <summary>
        /// Returns exception to status mapping method.
        /// </summary>
        Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        /// <summary>
        /// Returns feature progress notifier.
        /// </summary>
        IFeatureProgressNotifier FeatureProgressNotifier { get; }
        /// <summary>
        /// Returns scenario progress notifier provider method.
        /// </summary>
        Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        /// <summary>
        /// Returns LightBDD execution extensions.
        /// </summary>
        IExecutionExtensions ExecutionExtensions { get; }
        /// <summary>
        /// Returns LightBDD Configuration.
        /// </summary>
        LightBddConfiguration Configuration { get; }
    }
}