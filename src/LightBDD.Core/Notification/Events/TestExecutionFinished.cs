using LightBDD.Core.Execution;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when all tests execution is finished.
    /// </summary>
    public class TestExecutionFinished : ProgressEvent
    {
        /// <summary>
        /// Test execution result.
        /// </summary>
        public ITestExecutionResult Result { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TestExecutionFinished(EventTime time, ITestExecutionResult result) : base(time)
        {
            Result = result;
        }
    }
}