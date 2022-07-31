using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification.Events;

/// <summary>
/// Event raised when step gets file attached
/// </summary>
public class StepFileAttached : ProgressEvent
{
    /// <summary>
    /// Step.
    /// </summary>
    public IStepInfo Step { get; }
        
    /// <summary>
    /// Attachment.
    /// </summary>
    public FileAttachment Attachment { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public StepFileAttached(EventTime time, IStepInfo step, FileAttachment attachment) : base(time)
    {
        Step = step;
        Attachment = attachment;
    }
}