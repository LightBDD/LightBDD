﻿using System.Threading;
using LightBDD.Core.Results;

namespace LightBDD.Framework.Notification.Implementation
{
    internal class ProgressManager
    {
        private readonly object _sync = new();
        private readonly AsyncLocal<int?> _currentScenarioId = new();
        private int _totalScenarios;
        private int _finishedScenarios;
        private int _pendingScenarios;
        private int _failedScenarios;

        public ProgressState GetProgress()
        {
            lock (_sync)
                return new ProgressState(_finishedScenarios, _pendingScenarios, _failedScenarios, _currentScenarioId.Value);
        }

        public void StartNewScenario()
        {
            _currentScenarioId.Value = Interlocked.Increment(ref _totalScenarios);
            lock (_sync)
                ++_pendingScenarios;
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
            _currentScenarioId.Value = null;
        }
    }
}