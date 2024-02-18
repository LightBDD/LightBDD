using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing scenario metadata.
    /// </summary>
    public interface IScenarioInfo : IMetadataInfo
    {
        /// <summary>
        /// Returns scenario name.
        /// </summary>
        //TODO: remove in 4.x
        new INameInfo Name { get; }
        /// <summary>
        /// The scenario parent feature.
        /// </summary>
        IFeatureInfo Parent { get; }
        /// <summary>
        /// Returns scenario labels or empty collection if none specified.
        /// </summary>
        IEnumerable<string> Labels { get; }
        /// <summary>
        /// Returns scenario categories or empty collection if none specified.
        /// </summary>
        IEnumerable<string> Categories { get; }
        /// <summary>
        /// Returns scenario description or <c>null</c> if none provided.
        /// </summary>
        string Description { get; }
    }
}