using System;

namespace LightBDD.Core.Execution.Results
{
    public class ExecutionTime
    {
        public ExecutionTime(DateTimeOffset start, TimeSpan duration)
        {
            Start = start;
            Duration = duration;
        }

        public DateTimeOffset Start { get; }
        public TimeSpan Duration { get; }

        public DateTimeOffset End => Start + Duration;
    }
}