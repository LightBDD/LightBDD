using System;
using System.IO;
using System.Threading.Tasks;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;
using LightBDD.Framework.Reporting.Implementation;

namespace LightBDD.Framework.Reporting;

/// <summary>
/// File Attachments Manager
/// </summary>
public class FileAttachmentsManager : IFileAttachmentsManager
{
    private bool _directoryCreated;

    /// <summary>
    /// Directory where attachments are stored
    /// </summary>
    public string AttachmentsDirectory { get; }

    /// <summary>
    /// Initializes the File Attachments Manager with <paramref name="attachmentsDirectory"/>
    /// </summary>
    /// <param name="attachmentsDirectory">Attachments directory. If starts with <c>~</c>, it would be resolved to <c>AppContext.BaseDirectory</c>.</param>
    public FileAttachmentsManager(string attachmentsDirectory)
    {
        AttachmentsDirectory = FilePathHelper.ResolveAbsolutePath(attachmentsDirectory);
    }

    /// <inheritdoc />
    public Task<FileAttachment> CreateFromFile(string name, string filePath, bool removeOriginalFile = true)
    {
        var fileExtension = SanitizeExtension(Path.GetExtension(filePath));
        var destinationFile = GetAttachmentFile(fileExtension);
        var destinationFilePath = CreateDestinationFilePath(destinationFile);

        if (removeOriginalFile)
            File.Move(filePath, destinationFilePath);
        else
            File.Copy(filePath, destinationFilePath);

        return Task.FromResult(new FileAttachment(name, destinationFilePath, destinationFile));
    }

    /// <inheritdoc />
    public async Task<FileAttachment> CreateFromStream(string name, string fileExtension, Func<Stream, Task> writeStreamFn)
    {
        fileExtension = SanitizeExtension(fileExtension);
        var destinationFile = GetAttachmentFile(fileExtension);
        var destinationFilePath = CreateDestinationFilePath(destinationFile);

        using var stream = File.OpenWrite(destinationFilePath);
        await writeStreamFn(stream);

        return new(name, destinationFilePath, destinationFile);
    }

    private string SanitizeExtension(string fileExtension)
    {
        if (string.IsNullOrWhiteSpace(fileExtension))
            return ".dat";

        fileExtension = fileExtension.Trim().ToLowerInvariant();

        return fileExtension[0] == '.'
            ? fileExtension
            : $".{fileExtension}";
    }

    private string GetAttachmentFile(string extension) => $"{Guid.NewGuid()}{extension}";

    private string CreateDestinationFilePath(string destinationFile)
    {
        if (!_directoryCreated)
            EnsureDirectoryCreated();
        return Path.Combine(AttachmentsDirectory, destinationFile);
    }

    private void EnsureDirectoryCreated()
    {
        lock (AttachmentsDirectory)
        {
            if (_directoryCreated)
                return;
            Directory.CreateDirectory(AttachmentsDirectory);
            _directoryCreated = true;
        }
    }
}