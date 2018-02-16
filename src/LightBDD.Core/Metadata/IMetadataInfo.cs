namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing metadata (such as feature, scenario or step)
    /// </summary>
    public interface IMetadataInfo
    {
        /// <summary>
        /// Unique Id assigned at runtime to the metadata.
        /// Please note that the same metadata may have different Ids in consecutive test runs.
        /// </summary>
        string RuntimeId { get; }
        /// <summary>
        /// Returns name of metadata.
        /// </summary>
        INameInfo Name { get; }
    }
}
