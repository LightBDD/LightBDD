using System;
using System.Diagnostics;

namespace LightBDD.Core.Execution.Implementation
{
    class DefaultExecutionTimer : IExecutionTimer
    {
        private readonly Stopwatch _watch;
        private readonly DateTimeOffset _start;

        private DefaultExecutionTimer()
        {
            _start = DateTimeOffset.UtcNow;
            _watch = Stopwatch.StartNew();
        }

        public static IExecutionTimer StartNew() => new DefaultExecutionTimer();

        public EventTime GetTime() => new EventTime(_start, _watch.Elapsed);
    }
}