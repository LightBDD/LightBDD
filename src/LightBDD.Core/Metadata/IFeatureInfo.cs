using System;
using System.Collections.Generic;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing feature metadata.
    /// </summary>
    public interface IFeatureInfo : IMetadataInfo
    {
        /// <summary>
        /// Returns feature name.
        /// </summary>
        //TODO: remove in 4.x
        new INameInfo Name { get; }
        /// <summary>
        /// Returns feature labels or empty collection if none provided.
        /// </summary>
        IEnumerable<string> Labels { get; }
        /// <summary>
        /// Returns feature description or <c>null</c> if none provided.
        /// </summary>
        string Description { get; }

        Type FeatureType { get; }
    }
}