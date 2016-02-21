using System;
using LightBDD.Core.Execution.Results;
using LightBDD.Core.Formatting;
using LightBDD.Core.Notification;

namespace LightBDD.Core.Extensibility
{
    public interface IIntegrationContext
    {
        IMetadataProvider MetadataProvider { get; }
        INameFormatter NameFormatter { get; }
        Func<Exception,ExecutionStatus> ExceptionToStatusMapper { get; }
        IProgressNotifier ProgressNotifier { get; }
    }
}