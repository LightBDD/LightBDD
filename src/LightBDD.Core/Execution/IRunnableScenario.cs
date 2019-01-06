using System.Threading.Tasks;

namespace LightBDD.Core.Execution
{
    public interface IRunnableScenario
    {
        /// <summary>
        /// Executes scenario.
        /// Any exceptions thrown in scenario steps will be wrapped in <see cref="ScenarioExecutionException"/>. Code calling this method can rethrow the original exception by calling <code>ex.GetOriginal().Throw()</code>
        /// </summary>
        /// <exception cref="ScenarioExecutionException">Thrown when any exception occurs during execution.</exception>
        Task ExecuteAsync();
    }
}