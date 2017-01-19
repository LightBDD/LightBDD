using System.Collections.Generic;

namespace LightBDD.Core.Extensibility.Execution
{
    public interface IExecutionExtensions
    {
        IEnumerable<IScenarioExecutionExtension> ScenarioExecutionExtensions { get; }
        IEnumerable<IStepExecutionExtension> StepExecutionExtensions { get; }
    }
}