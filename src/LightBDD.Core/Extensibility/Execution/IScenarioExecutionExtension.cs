using System;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface describing scenario execution extension that can be used by LightBDD to decorate scenario execution.
    /// </summary>
    [Obsolete("Use IScenarioDecorator instead", true)]
    public interface IScenarioExecutionExtension
    {
        /// <summary>
        /// Method allowing to decorate scenario invocation specified by <paramref name="scenarioInvocation"/>.
        /// </summary>
        /// <param name="scenario">Scenario that is being executed.</param>
        /// <param name="scenarioInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation);
    }
}