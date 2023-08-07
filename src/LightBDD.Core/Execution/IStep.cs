using System;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Metadata;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Interface describing current step, providing step information details and ability to comment it.
    /// </summary>
    public interface IStep : IExecutable
    {
        /// <summary>
        /// Step information details.
        /// </summary>
        IStepInfo Info { get; }
        /// <summary>
        /// Annotates step with comment.
        /// It is possible to comment step many times.
        /// </summary>
        /// <param name="comment">Comment.</param>
        void Comment(string comment);
        /// <summary>
        /// Returns the dependency resolver used by this step.
        /// </summary>
        //TODO: remove in favor of ctor injection
        IDependencyResolver DependencyResolver { get; }
        /// <summary>
        /// Returns the context used by this step (or null if none were provided).
        /// </summary>
        object Context { get; }

        /// <summary>
        /// Adds the file attachment to the step.
        /// </summary>
        /// <param name="createAttachmentFn">Function creating file attachment by using provided attachments manager.</param>
        /// <returns></returns>
        Task AttachFile(Func<IFileAttachmentsManager, Task<FileAttachment>> createAttachmentFn);
    }
}