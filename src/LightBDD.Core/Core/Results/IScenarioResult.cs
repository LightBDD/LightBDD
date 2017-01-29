using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results
{
    /// <summary>
    /// Interface describing scenario test result.
    /// </summary>
    public interface IScenarioResult
    {
        /// <summary>
        /// Returns scenario details.
        /// </summary>
        IScenarioInfo Info { get; }
        /// <summary>
        /// Returns scenario execution status.
        /// </summary>
        ExecutionStatus Status { get; }
        /// <summary>
        /// Returns status details.
        /// It will contain details for all bypassed, ignored or failed steps.
        /// It may be null if no additional details are provided.
        /// </summary>
        string StatusDetails { get;  }
        /// <summary>
        /// Returns scenario execution time.
        /// </summary>
        ExecutionTime ExecutionTime { get; }
        /// <summary>
        /// Returns results of steps executed within this scenario.
        /// </summary>
        /// <returns>Collection of step results.</returns>
        IEnumerable<IStepResult> GetSteps();
    }
}