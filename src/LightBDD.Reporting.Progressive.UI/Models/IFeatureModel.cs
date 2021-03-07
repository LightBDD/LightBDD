using System;
using System.Collections.Generic;

namespace LightBDD.Reporting.Progressive.UI.Models
{
    public interface IFeatureModel
    {
        Guid Id { get; }
        string Description { get; }
        IReadOnlyList<string> Labels { get; }
        INameInfo Name { get; }
        IReadOnlyList<IScenarioModel> Scenarios { get; }
    }
}