using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when scenario execution is about to start.<br/>
    /// At the point of this event the scenario context nor dependency injection scope is not initialized yet.
    /// </summary>
    public class ScenarioStarting : ProgressEvent
    {
        /// <summary>
        /// Scenario.
        /// </summary>
        public IScenarioInfo Scenario { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScenarioStarting(EventTime time, IScenarioInfo scenario) : base(time)
        {
            Scenario = scenario;
        }
    }
}