using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results
{
    /// <summary>
    /// Interface describing feature test result.
    /// </summary>
    public interface IFeatureResult
    {
        /// <summary>
        /// Returns feature details.
        /// </summary>
        IFeatureInfo Info { get; }
        /// <summary>
        /// Returns results of scenarios executed within this feature.
        /// </summary>
        /// <returns>Collection of scenario results.</returns>
        IEnumerable<IScenarioResult> GetScenarios();
    }
}