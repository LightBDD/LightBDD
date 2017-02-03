using System;
using System.Collections.Generic;
using LightBDD.Core.Results;

namespace LightBDD.Reporting
{
    /// <summary>
    /// Class providing execution time summary.
    /// </summary>
    public class ExecutionTimeSummary
    {
        /// <summary>
        /// Returns execution start.
        /// </summary>
        public DateTimeOffset Start { get; }
        /// <summary>
        /// Returns execution end.
        /// </summary>
        public DateTimeOffset End { get; }
        /// <summary>
        /// Returns real execution duration (End - Start)
        /// </summary>
        public TimeSpan Duration => End - Start;
        /// <summary>
        /// Returns aggregated execution duration.
        /// </summary>
        public TimeSpan Aggregated { get; }
        /// <summary>
        /// Returns average execution duration.
        /// </summary>
        public TimeSpan Average { get; }

        /// <summary>
        /// Constructor allowing to setup all properties.
        /// </summary>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <param name="aggregated">Aggregated.</param>
        /// <param name="average">Average.</param>
        public ExecutionTimeSummary(DateTimeOffset start, DateTimeOffset end, TimeSpan aggregated, TimeSpan average)
        {
            Start = start;
            End = end;
            Aggregated = aggregated;
            Average = average;
        }

        /// <summary>
        /// Default constructor initializing <see cref="Start"/> and <see cref="End"/> with <see cref="DateTimeOffset.UtcNow"/> and <see cref="Average"/> and <see cref="Aggregated"/> with <see cref="TimeSpan.Zero"/>.
        /// </summary>
        public ExecutionTimeSummary()
        {
            Start = End = DateTimeOffset.UtcNow;
            Average = Aggregated = TimeSpan.Zero;
        }

        /// <summary>
        /// Calculates <see cref="ExecutionTimeSummary"/> for all <see cref="ExecutionTime"/> instances provided in <paramref name="times"/> argument.
        /// The <see cref="Start"/> is set to the lowest <see cref="ExecutionTime.Start"/> value from provided <paramref name="times"/>.
        /// The <see cref="End"/> is set to the highest <see cref="ExecutionTime.End"/> value from provided <paramref name="times"/>.
        /// The <see cref="Aggregated"/> is set to the sum of all <see cref="ExecutionTime.Duration"/> values from provided <paramref name="times"/>.
        /// The <see cref="Average"/> is set to the <see cref="Aggregated"/> divided by number of values from provided <paramref name="times"/>.
        /// </summary>
        /// <param name="times">Times to calculate summary for.</param>
        /// <returns><see cref="ExecutionTimeSummary"/> object.</returns>
        public static ExecutionTimeSummary Calculate(IEnumerable<ExecutionTime> times)
        {
            var low = DateTimeOffset.MaxValue;
            var high = DateTimeOffset.MinValue;
            var aggregated = TimeSpan.Zero;
            int count = 0;

            foreach (var time in times)
            {
                if (low > time.Start)
                    low = time.Start;
                if (high < time.End)
                    high = time.End;

                aggregated += time.Duration;
                count++;
            }

            return count == 0
                ? new ExecutionTimeSummary()
                : new ExecutionTimeSummary(low, high, aggregated, TimeSpan.FromTicks(aggregated.Ticks / count));
        }
    }
}