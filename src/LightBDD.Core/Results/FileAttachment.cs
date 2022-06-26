namespace LightBDD.Core.Results;

/// <summary>
/// File attachment
/// </summary>
public class FileAttachment
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public FileAttachment(string name, string filePath)
    {
        Name = name;
        FilePath = filePath;
    }

    /// <summary>
    /// Attachment name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Attachment path
    /// </summary>
    public string FilePath { get; }
}