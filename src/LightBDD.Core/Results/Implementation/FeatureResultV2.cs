#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results.Implementation;

//TODO: replace V1
internal class FeatureResultV2 : IFeatureResult
{
    private readonly IReadOnlyList<IScenarioResult> _scenarios;

    public FeatureResultV2(IFeatureInfo info, IReadOnlyList<IScenarioResult> scenarios)
    {
        _scenarios = scenarios;
        Info = info;
    }

    public IFeatureInfo Info { get; }
    public IEnumerable<IScenarioResult> GetScenarios() { return _scenarios; }

    public override string ToString() => Info.ToString();
}