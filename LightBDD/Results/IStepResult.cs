using System;

namespace LightBDD.Results
{
    /// <summary>
    /// Interface describing scenario step test result.
    /// </summary>
    public interface IStepResult
    {
        /// <summary>
        /// Step name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Step number.
        /// </summary>
        int Number { get; }

        /// <summary>
        /// Step status.
        /// </summary>
        ResultStatus Status { get; }

        /// <summary>
        /// Status details.
        /// It is useful for ignored or failed tests.
        /// It may be null if no additional details are provided.
        /// </summary>
        string StatusDetails { get; }

        /// <summary>
        /// Step execution time.
        /// Null if step was not run.
        /// </summary>
        TimeSpan? ExecutionTime { get; }

        /// <summary>
        /// Time when step method execution started.
        /// Null if step was not run.
        /// </summary>
        DateTimeOffset? ExecutionStart { get; }

        /// <summary>
        /// Returns step name details.
        /// </summary>
        IStepName StepName { get; }
    }
}