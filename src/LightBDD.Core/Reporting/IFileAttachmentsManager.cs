using System;
using System.IO;
using System.Threading.Tasks;
using LightBDD.Core.Results;

namespace LightBDD.Core.Reporting;

/// <summary>
/// Manager responsible for creating File Attachments that can be associated with scenario steps.
/// </summary>
public interface IFileAttachmentsManager
{
    /// <summary>
    /// Creates attachment from existing file.<br/>
    /// The attachment is stored in the pre-configured location.<br/>
    /// Caller can choose if the original file should be removed after (default option) or preserved.
    /// </summary>
    /// <param name="name">Attachment name</param>
    /// <param name="filePath">Attachment file location</param>
    /// <param name="removeOriginalFile">Shall the original file be removed after attachment creation?</param>
    /// <returns>File attachment</returns>
    Task<FileAttachment> CreateFromFile(string name, string filePath, bool removeOriginalFile = true);

    /// <summary>
    /// Creates attachment by using write stream.<br/>
    /// The attachment is stored in the pre-configured location.<br/>
    /// </summary>
    /// <param name="name">Attachment name</param>
    /// <param name="fileExtension">File extension</param>
    /// <param name="writeStreamFn">Function providing write stream to the attachment</param>
    /// <returns>File attachment</returns>
    Task<FileAttachment> CreateFromStream(string name, string fileExtension, Func<Stream, Task> writeStreamFn);
}