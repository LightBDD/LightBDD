using LightBDD.Core.Execution;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Progress event.
    /// </summary>
    public class ProgressEvent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ProgressEvent(EventTime time)
        {
            Time = time;
        }

        /// <summary>
        /// Event time.
        /// </summary>
        public EventTime Time { get; }
    }
}