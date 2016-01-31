using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Results
{
    public interface IStepResult
    {
        IStepInfo Info { get; }
        int Number { get; }
        ExecutionStatus Status { get; }
        string StatusDetails { get; }
    }
}