using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results
{
    public interface IFeatureResult
    {
        IFeatureInfo Info { get; }
        IEnumerable<IScenarioResult> GetScenarios();
    }
}