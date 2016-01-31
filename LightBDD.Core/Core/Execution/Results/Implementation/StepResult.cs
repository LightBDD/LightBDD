using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Results.Implementation
{
    internal class StepResult : IStepResult
    {
        public StepResult(IStepInfo info, int number)
        {
            Info = info;
            Number = number;
        }

        public IStepInfo Info { get; private set; }
        public int Number { get; private set; }
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }

        public void SetStatus(ExecutionStatus status, string details = null)
        {
            Status = status;
            StatusDetails = details;
        }
    }
}