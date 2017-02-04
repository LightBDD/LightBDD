using System.Collections.Generic;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface specifying extensions that should be used by LightBDD.
    /// </summary>
    public interface IExecutionExtensions
    {
        /// <summary>
        /// Collection of scenario execution extensions.
        /// </summary>
        IEnumerable<IScenarioExecutionExtension> ScenarioExecutionExtensions { get; }
        /// <summary>
        /// Collection of step execution extensions.
        /// </summary>
        IEnumerable<IStepExecutionExtension> StepExecutionExtensions { get; }
    }
}