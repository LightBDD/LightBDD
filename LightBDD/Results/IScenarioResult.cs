using System;
using System.Collections.Generic;

namespace LightBDD.Results
{
    /// <summary>
    /// Interface describing scenario test result.
    /// </summary>
    public interface IScenarioResult
    {
        /// <summary>
        /// [Label] attribute associated to scenario.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Scenario name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Scenario status.
        /// </summary>
        ResultStatus Status { get; }

        /// <summary>
        /// Status details.
        /// It is useful for ignored or failed tests.
        /// It may be null if no additional details are provided.
        /// </summary>
        string StatusDetails { get; }

        /// <summary>
        /// Scenario steps.
        /// </summary>
        IEnumerable<IStepResult> Steps { get; }

        /// <summary>
        /// Scenario execution time.
        /// Null if scenario was not run.
        /// </summary>
        TimeSpan? ExecutionTime { get; }

        /// <summary>
        /// Time when scenario execution started.
        /// Null if scenario was not run.
        /// </summary>
        DateTimeOffset? ExecutionStart { get; }
    }
}