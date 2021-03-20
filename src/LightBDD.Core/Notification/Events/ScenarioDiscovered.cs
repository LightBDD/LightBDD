using LightBDD.Core.Execution;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Notification.Events
{
    /// <summary>
    /// Event raised when scenario is discovered.
    /// </summary>
    public class ScenarioDiscovered : ProgressEvent
    {
        /// <summary>
        /// Scenario.
        /// </summary>
        public IScenarioInfo Scenario { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScenarioDiscovered(EventTime time, IScenarioInfo scenario) : base(time)
        {
            Scenario = scenario;
        }
    }
}