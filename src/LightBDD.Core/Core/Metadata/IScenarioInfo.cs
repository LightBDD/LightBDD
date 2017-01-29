using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing scenario metadata.
    /// </summary>
    public interface IScenarioInfo
    {
        /// <summary>
        /// Returns scenario name.
        /// </summary>
        INameInfo Name { get; }
        /// <summary>
        /// Returns scenario labels or empty collection if none specified.
        /// </summary>
        IEnumerable<string> Labels { get; }
        /// <summary>
        /// Returns scenario categories or empty collection if none specified.
        /// </summary>
        IEnumerable<string> Categories { get; }
    }
}