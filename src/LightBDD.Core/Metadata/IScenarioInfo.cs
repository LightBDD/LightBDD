using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing scenario metadata.
    /// </summary>
    public interface IScenarioInfo
    {
        /// <summary>
        /// Unique Id assigned at runtime.
        /// Please note that the same scenario may have different Ids in consecutive test runs.
        /// </summary>
        string RuntimeId { get; }
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