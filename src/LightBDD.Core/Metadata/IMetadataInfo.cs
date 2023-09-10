namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing metadata (such as feature, scenario or step)
    /// </summary>
    public interface IMetadataInfo : IRuntimeObjectInfo
    {
        /// <summary>
        /// Returns name of the metadata.
        /// </summary>
        INameInfo Name { get; }

    }
}