using System;
using System.Collections.Generic;
using LightBDD.Reporting.Progressive.UI.Utils;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    public interface IStepModel : IObservableStateChange
    {
        Guid Id { get; }
        Guid ParentId { get; }
        string GroupPrefix { get; }
        int Number { get; }
        IStepNameInfo Name { get; }
        IReadOnlyList<IStepModel> SubSteps { get; }
    }
}