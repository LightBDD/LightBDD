using System;
using System.Diagnostics;
using LightBDD.Core.Results;

namespace LightBDD.Core.Execution.Implementation
{
    internal class ExecutionTimeWatch
    {
        private readonly Stopwatch _watch = new Stopwatch();
        private DateTimeOffset _start;

        public static ExecutionTimeWatch StartNew()
        {
            return new ExecutionTimeWatch().Start();
        }

        private ExecutionTimeWatch Start()
        {
            _start = DateTimeOffset.UtcNow;
            _watch.Restart();
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