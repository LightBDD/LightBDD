using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when given step is commented.
    /// </summary>
    public class StepCommented : ProgressEvent
    {
        /// <summary>
        /// Step.
        /// </summary>
        public IStepInfo Step { get; }

        /// <summary>
        /// Comment.
        /// </summary>
        public string Comment { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StepCommented(EventTime time, IStepInfo step, string comment) : base(time)
        {
            Step = step;
            Comment = comment;
        }
    }
}