using LightBDD.Core.Metadata;
using LightBDD.Core.Metadata.Implementation;

namespace LightBDD.Core.Execution.Results.Implementation
{
    internal class StepResult : IStepResult
    {
        private readonly StepInfo _info;

        public StepResult(StepInfo info)
        {
            _info = info;
        }

        public IStepInfo Info => _info;
        public ExecutionStatus Status { get; private set; }
        public string StatusDetails { get; private set; }
        public ExecutionTime ExecutionTime { get; private set; }

        public void SetStatus(ExecutionStatus status, string details = null)
        {
            Status = status;
            StatusDetails = details;
        }

        public void UpdateName(INameParameterInfo[] parameters)
        {
            _info.UpdateName(parameters);
        }

        public void SetExecutionTime(ExecutionTime executionTime)
        {
            ExecutionTime = executionTime;
        }
    }
}