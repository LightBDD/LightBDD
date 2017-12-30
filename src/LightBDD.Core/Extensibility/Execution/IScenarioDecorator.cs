using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface describing scenario execution extension that can be used by LightBDD to decorate scenario execution.
    /// </summary>
    public interface IScenarioDecorator
    {
        /// <summary>
        /// Method allowing to decorate scenario invocation specified by <paramref name="scenarioInvocation"/>.<br/>
        /// Any exceptions thrown from <paramref name="scenarioInvocation"/> will be wrapped in <see cref="ScenarioExecutionException"/>, which means that if this method suppose to handle exceptions, it will have to use <see cref="Exception.InnerException"/> property of <see cref="ScenarioExecutionException"/>.
        /// It is allowed to throw any exception type from this method.
        /// </summary>
        /// <param name="scenario">Scenario that is being executed.</param>
        /// <param name="scenarioInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation);
    }
}