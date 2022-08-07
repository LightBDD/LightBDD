using LightBDD.Core.Execution;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when given step execution is finished.
    /// </summary>
    public class StepFinished : ProgressEvent
    {
        /// <summary>
        /// Step result.
        /// </summary>
        public IStepResult Result { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StepFinished(EventTime time, IStepResult result) : base(time)
        {
            Result = result;
        }
    }
}