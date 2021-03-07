using System;
using System.Collections.Generic;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    public interface IScenarioModel
    {
        Guid Id { get; }
        Guid FeatureId { get; }
        IReadOnlyList<string> Categories { get; }
        IReadOnlyList<string> Labels { get; }
        INameInfo Name { get; }
        IReadOnlyList<IStepModel> Steps { get; }
    }
}