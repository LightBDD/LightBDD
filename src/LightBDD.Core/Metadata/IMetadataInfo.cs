using System;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing metadata (such as feature, scenario or step)
    /// </summary>
    public interface IMetadataInfo
    {
        /// <summary>
        /// Returns name of the metadata.
        /// </summary>
        INameInfo Name { get; }
        /// <summary>
        /// Unique Id assigned at runtime to the metadata.
        /// Please note that the same metadata will have different Ids in consecutive test runs.
        /// </summary>
        Guid RuntimeId { get; }
    }
}