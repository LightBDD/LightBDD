using System;
using System.Diagnostics;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Default implementation of <see cref="IExecutionTimer"/>.
    /// </summary>
    public class DefaultExecutionTimer : IExecutionTimer
    {
        private readonly Stopwatch _watch;
        private readonly DateTimeOffset _start;

        private DefaultExecutionTimer()
        {
            _start = DateTimeOffset.UtcNow;
            _watch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Starts new timer
        /// </summary>
        /// <returns>Timer</returns>
        public static IExecutionTimer StartNew() => new DefaultExecutionTimer();

        /// <summary>
        /// Gets current execution time.
        /// </summary>
        /// <returns><see cref="EventTime"/> instance</returns>
        public EventTime GetTime() => new(_start, _watch.Elapsed);
    }
}