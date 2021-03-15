using System;
using System.Collections.Generic;
using LightBDD.Notification.Jsonl.Models;
using LightBDD.Reporting.Progressive.UI.Utils;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    public interface IScenarioModel : IObservableStateChange
    {
        Guid Id { get; }
        Guid FeatureId { get; }
        IReadOnlyList<string> Categories { get; }
        IReadOnlyList<string> Labels { get; }
        INameInfo Name { get; }
        IReadOnlyList<IStepModel> Steps { get; }
        ExecutionStatus Status { get; }
        string StatusDetails { get; }
        TimeSpan? ExecutionTime { get; }
    }
}