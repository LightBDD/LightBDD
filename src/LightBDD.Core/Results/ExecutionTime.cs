using System;
using System.Diagnostics;

namespace LightBDD.Core.Results
{
    /// <summary>
    /// Class describing execution time.
    /// </summary>
    [DebuggerStepThrough]
    public class ExecutionTime
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="start">Execution start.</param>
        /// <param name="duration">Execution duration.</param>
        public ExecutionTime(DateTimeOffset start, TimeSpan duration)
        {
            Start = start;
            Duration = duration;
        }

        /// <summary>
        /// Returns execution start.
        /// </summary>
        public DateTimeOffset Start { get; }
        /// <summary>
        /// Returns execution duration.
        /// </summary>
        public TimeSpan Duration { get; }
        /// <summary>
        /// Returns execution end.
        /// </summary>
        public DateTimeOffset End => Start + Duration;
    }
}