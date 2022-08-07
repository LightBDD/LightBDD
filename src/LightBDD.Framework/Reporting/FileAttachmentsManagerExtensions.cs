using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Reporting
{
    /// <summary>
    /// Extension methods for <see cref="IFileAttachmentsManager"/>
    /// </summary>
    public static class FileAttachmentsManagerExtensions
    {
        /// <summary>
        /// Creates attachment from in-memory content.<br/>
        /// The attachment is stored in the pre-configured location.<br/>
        /// </summary>
        /// <param name="manager">File attachments manager</param>
        /// <param name="name">Attachment name</param>
        /// <param name="fileExtension">File extension</param>
        /// <param name="content">Attachment content</param>
        /// <returns>File attachment</returns>
        public static async Task<FileAttachment> CreateFromData(this IFileAttachmentsManager manager, string name, string fileExtension, byte[] content)
        {
            Task WriteContent(Stream stream) => stream.WriteAsync(content, 0, content.Length);

            return await manager.CreateFromStream(name, fileExtension, WriteContent);
        }

        /// <summary>
        /// Creates attachment from text content.<br/>
        /// The attachment is stored in the pre-configured location.<br/>
        /// </summary>
        /// <param name="manager">File attachments manager</param>
        /// <param name="name">Attachment name</param>
        /// <param name="fileExtension">File extension</param>
        /// <param name="content">Attachment content</param>
        /// <param name="encoding">Text encoding</param>
        /// <returns>File attachment</returns>
        public static async Task<FileAttachment> CreateFromText(this IFileAttachmentsManager manager, string name, string fileExtension, string content, Encoding encoding)
        {
            async Task WriteContent(Stream stream)
            {
                using var writer = new StreamWriter(stream, encoding);
                await writer.WriteAsync(content);
            }
            return await manager.CreateFromStream(name, fileExtension, WriteContent);
        }
    }
}
