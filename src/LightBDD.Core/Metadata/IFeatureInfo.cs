using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing feature metadata.
    /// </summary>
    public interface IFeatureInfo
    {
        /// <summary>
        /// Unique Id assigned at runtime.
        /// Please note that the same feature may have different Ids in consecutive test runs.
        /// </summary>
        string RuntimeId { get; }
        /// <summary>
        /// Returns feature name.
        /// </summary>
        INameInfo Name { get; }
        /// <summary>
        /// Returns feature labels or empty collection if none provided.
        /// </summary>
        IEnumerable<string> Labels { get; }
        /// <summary>
        /// Returns feature description or <c>null</c> if none provided.
        /// </summary>
        string Description { get; }
    }
}