using System.Diagnostics;
using System.Threading;
using LightBDD.Core.Results;

namespace LightBDD.Notification.Implementation
{
    [DebuggerStepThrough]
    internal class ProgressManager
    {
        private readonly object _sync = new object();
        private int _totalScenarios;
        private int _finishedScenarios;
        private int _pendingScenarios;
        private int _failedScenarios;

        public ProgressState GetProgress()
        {
            lock (_sync)
                return new ProgressState(_finishedScenarios, _pendingScenarios, _failedScenarios);
        }

        public int StartNewScenario()
        {
            var current = Interlocked.Increment(ref _totalScenarios);
            lock (_sync)
                ++_pendingScenarios;
            return current;
        }

        public void CaptureScenarioResult(ExecutionStatus scenarioStatus)
        {
            lock (_sync)
            {
                ++_finishedScenarios;
                if (scenarioStatus == ExecutionStatus.Failed)
                    ++_failedScenarios;
                --_pendingScenarios;
            }
        }

        public void FinishScenario()
        {
        }
    }
}