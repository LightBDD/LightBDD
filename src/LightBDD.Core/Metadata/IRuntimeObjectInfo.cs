namespace LightBDD.Core.Metadata;

/// <summary>
/// Interface describing runtime object having and unique Id
/// </summary>
public interface IRuntimeObjectInfo
{
    /// <summary>
    /// Unique Id assigned at runtime to the metadata.
    /// Please note that the same metadata may have different Ids in consecutive test runs.
    /// </summary>
    string RuntimeId { get; }
}