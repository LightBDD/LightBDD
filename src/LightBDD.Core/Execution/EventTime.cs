using System;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Represents point in time when given event occurred.
    /// </summary>
    public readonly struct EventTime
    {
        /// <summary>
        /// Time when measurement started.
        /// </summary>
        public DateTimeOffset Start { get; }
        /// <summary>
        /// Offset since <seealso cref="Start"/> when given event occurred.
        /// </summary>
        public TimeSpan Offset { get; }

        /// <summary>
        /// Event time.
        /// </summary>
        public DateTimeOffset Time => Start + Offset;

        /// <summary>
        /// Creates event time from <paramref name="start"/> and <paramref name="offset"/>.
        /// </summary>
        /// <param name="start">Time when measurement started.</param>
        /// <param name="offset">Offset from measurement start.</param>
        public EventTime(DateTimeOffset start, TimeSpan offset)
        {
            Start = start;
            Offset = offset;
        }

        /// <summary>
        /// Determine <see cref="ExecutionTime"/> based on current and <paramref name="other"/> points in time.<br/>
        /// </summary>
        public ExecutionTime GetExecutionTime(EventTime other)
        {
            return Time < other.Time
                ? new ExecutionTime(Time, other.Time - Time)
                : new ExecutionTime(other.Time, Time - other.Time);
        }
    }
}