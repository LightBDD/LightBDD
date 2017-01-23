using System;
using LightBDD.Core.Extensibility.Execution;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;
using LightBDD.Core.Results;

namespace LightBDD.Core.Extensibility
{
    public interface IIntegrationContext
    {
        IMetadataProvider MetadataProvider { get; }
        INameFormatter NameFormatter { get; }
        Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        IFeatureProgressNotifier FeatureProgressNotifier { get; }
        Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        IExecutionExtensions ExecutionExtensions { get; }
    }
}