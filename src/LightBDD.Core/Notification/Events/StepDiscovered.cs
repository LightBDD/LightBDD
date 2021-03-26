using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when step is discovered.
    /// </summary>
    public class StepDiscovered : ProgressEvent
    {
        /// <summary>
        /// Step.
        /// </summary>
        public IStepInfo Step { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public StepDiscovered(EventTime time, IStepInfo step) : base(time)
        {
            Step = step;
        }
    }
}