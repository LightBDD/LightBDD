using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Interface describing current scenario.
    /// </summary>
    public interface IScenario : IExecutable
    {
        /// <summary>
        /// Scenario information details.
        /// </summary>
        IScenarioInfo Info { get; }
    }
}