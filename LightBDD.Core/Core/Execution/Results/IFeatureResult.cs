using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Results
{
    public interface IFeatureResult
    {
        IFeatureInfo Info { get; }
        IEnumerable<IScenarioResult> GetScenarios();
    }
}