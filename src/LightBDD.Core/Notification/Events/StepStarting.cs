using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when step execution is about to start.<br/>
    /// At the point of this event the step context nor dependency injection scope is not initialized yet.
    /// </summary>
    public class StepStarting : ProgressEvent
    {
        /// <summary>
        /// Step.
        /// </summary>
        public IStepInfo Step { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StepStarting(EventTime time, IStepInfo step) : base(time)
        {
            Step = step;
        }
    }
}