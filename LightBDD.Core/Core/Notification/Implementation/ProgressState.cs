namespace LightBDD.Core.Notification.Implementation
{
    internal struct ProgressState
    {
        public ProgressState(int finishedScenarios, int pendingScenarios, int failedScenarios)
        {
            FinishedScenarios = finishedScenarios;
            PendingScenarios = pendingScenarios;
            FailedScenarios = failedScenarios;
        }

        public int FinishedScenarios { get; }
        public int PendingScenarios { get; }
        public int FailedScenarios { get; }
    }
}