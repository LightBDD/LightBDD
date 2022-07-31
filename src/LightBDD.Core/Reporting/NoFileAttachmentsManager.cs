using System;
using System.IO;
using System.Threading.Tasks;
using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting;

/// <summary>
/// File attachments manager that is used when file attachments are disabled.
/// </summary>
public class NoFileAttachmentsManager : IFileAttachmentsManager
{
    private const string FileAttachmentsAreDisabled = "File Attachments are disabled";

    /// <summary>
    /// Default instance
    /// </summary>
    public static readonly NoFileAttachmentsManager Instance = new();

    /// <inheritdoc />
    public Task<FileAttachment> CreateFromFile(string name, string filePath, bool removeOriginalFile = true)
    {
        throw new NotSupportedException(FileAttachmentsAreDisabled);
    }

    /// <inheritdoc />
    public Task<FileAttachment> CreateFromStream(string name, string fileExtension, Func<Stream, Task> writeStreamFn)
    {
        throw new NotSupportedException(FileAttachmentsAreDisabled);
    }
}