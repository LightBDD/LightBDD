namespace LightBDD.Framework.Notification.Implementation
{
    internal struct ProgressState
    {
        public ProgressState(int finishedScenarios, int pendingScenarios, int failedScenarios, int? currentScenarioNumber)
        {
            FinishedScenarios = finishedScenarios;
            PendingScenarios = pendingScenarios;
            FailedScenarios = failedScenarios;
            CurrentScenarioNumber = currentScenarioNumber;
        }

        public int FinishedScenarios { get; }
        public int PendingScenarios { get; }
        public int FailedScenarios { get; }
        public int? CurrentScenarioNumber { get; }
    }
}