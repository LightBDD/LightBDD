namespace LightBDD.Core.Results;

/// <summary>
/// File attachment
/// </summary>
public class FileAttachment
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public FileAttachment(string name, string filePath, string relativePath)
    {
        Name = name;
        FilePath = filePath;
        RelativePath = relativePath;
    }

    /// <summary>
    /// Attachment name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Attachment file path
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Attachment relative path
    /// </summary>
    public string RelativePath { get; }
}