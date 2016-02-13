using System.Collections.Generic;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution.Results
{
    public interface IScenarioResult
    {
        IScenarioInfo Info { get; }
        ExecutionStatus Status { get; }
        string StatusDetails { get;  }
        ExecutionTime ExecutionTime { get; }
        IEnumerable<IStepResult> GetSteps();
    }
}