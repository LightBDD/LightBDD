using LightBDD.Core.Execution;
using LightBDD.Core.Results;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when given scenario execution is finished.
    /// </summary>
    public class ScenarioFinished : ProgressEvent
    {
        /// <summary>
        /// Scenario result.
        /// </summary>
        public IScenarioResult Result { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScenarioFinished(EventTime time, IScenarioResult result) : base(time)
        {
            Result = result;
        }
    }
}