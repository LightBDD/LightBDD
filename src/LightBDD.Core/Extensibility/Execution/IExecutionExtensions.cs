using System.Collections.Generic;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface specifying extensions that should be used by LightBDD.
    /// </summary>
    public interface IExecutionExtensions
    {
        /// <summary>
        /// Collection of scenario decorators.
        /// </summary>
        IEnumerable<IScenarioDecorator> ScenarioDecorators { get; }
        /// <summary>
        /// Collection of step decorators.
        /// </summary>
        IEnumerable<IStepDecorator> StepDecorators { get; }
    }
}