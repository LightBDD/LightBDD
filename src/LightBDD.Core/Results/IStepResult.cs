using System;
using System.Collections.Generic;
using LightBDD.Core.Metadata;
using LightBDD.Core.Results.Parameters;

namespace LightBDD.Core.Results
{
    /// <summary>
    /// Interface describing step test result.
    /// </summary>
    public interface IStepResult
    {
        /// <summary>
        /// Returns step details.
        /// </summary>
        IStepInfo Info { get; }
        /// <summary>
        /// Returns step execution status.
        /// </summary>
        ExecutionStatus Status { get; }
        /// <summary>
        /// Returns status details that contains reason for bypassed, ignored or failed steps.
        /// It may be null if no additional details are provided.
        /// </summary>
        string? StatusDetails { get; }
        /// <summary>
        /// Returns step execution time or null if step was not executed.
        /// </summary>
        ExecutionTime? ExecutionTime { get; }
        /// <summary>
        /// Returns step comments or empty collection if no comments were made.
        /// </summary>
        IEnumerable<string> Comments { get; }
        /// <summary>
        /// Returns exception caught during step execution or null if none occurred.
        /// </summary>
        Exception? ExecutionException { get; }
        /// <summary>
        /// Returns the step parameter evaluation results.
        /// </summary>
        IReadOnlyList<IParameterResult> Parameters { get; }
        /// <summary>
        /// Returns sub-steps if given step consists of any, or empty collection.
        /// </summary>
        IEnumerable<IStepResult> GetSubSteps();
    }
}