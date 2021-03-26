using LightBDD.Core.Execution;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when tests execution is starting.
    /// </summary>
    public class TestExecutionStarting : ProgressEvent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TestExecutionStarting(EventTime time) : base(time)
        {
        }
    }
}
