using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Results
{
    public interface IStepResult
    {
        IStepInfo Info { get; }
        ExecutionStatus Status { get; }
        string StatusDetails { get; }
        ExecutionTime ExecutionTime { get; }
        IEnumerable<string> Comments { get; }
    }
}