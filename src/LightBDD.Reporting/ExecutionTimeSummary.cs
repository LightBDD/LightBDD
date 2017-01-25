using System;
using System.Collections.Generic;
using LightBDD.Core.Results;

namespace LightBDD.Reporting
{
    public class ExecutionTimeSummary
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }
        public TimeSpan Duration => End - Start;
        public TimeSpan Aggregated { get; }
        public TimeSpan Average { get; }

        public ExecutionTimeSummary(DateTimeOffset start, DateTimeOffset end, TimeSpan aggregated, TimeSpan average)
        {
            Start = start;
            End = end;
            Aggregated = aggregated;
            Average = average;
        }

        public ExecutionTimeSummary()
        {
            Start = End = DateTimeOffset.UtcNow;
            Average = Aggregated = TimeSpan.Zero;
        }

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