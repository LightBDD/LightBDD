using System;
using System.Diagnostics;
using LightBDD.Core.Execution.Results;

namespace LightBDD.Core.Execution.Implementation
{
    class ExecutionTimeWatch
    {
        private DateTimeOffset _start;
        private Stopwatch _watch;

        public static ExecutionTimeWatch StartNew()
        {
            return new ExecutionTimeWatch().Start();
        }

        private ExecutionTimeWatch Start()
        {
            _start = DateTimeOffset.UtcNow;
            _watch = Stopwatch.StartNew();
            return this;
        }

        public ExecutionTimeWatch Stop()
        {
            _watch.Stop();
            return this;
        }

        public ExecutionTime GetTime()
        {
            return new ExecutionTime(_start, _watch.Elapsed);
        }
    }
}