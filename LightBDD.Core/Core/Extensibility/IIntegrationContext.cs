using System;
using LightBDD.Configuration;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Extensibility
{
    //TODO: rework interface to return read only configuration (it will add more flexibility in future)
    public interface IIntegrationContext
    {
        IMetadataProvider MetadataProvider { get; }
        INameFormatter NameFormatter { get; }
        Func<Exception, ExecutionStatus> ExceptionToStatusMapper { get; }
        IFeatureProgressNotifier FeatureProgressNotifier { get; }
        Func<object, IScenarioProgressNotifier> ScenarioProgressNotifierProvider { get; }
        //TODO: rework the type to be read only
        ExecutionExtensionsConfiguration ExecutionExtensions { get; }
    }
}