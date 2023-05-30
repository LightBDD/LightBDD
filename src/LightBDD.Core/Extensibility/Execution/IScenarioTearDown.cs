using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface providing <seealso cref="OnScenarioTearDown"/> method to execute additional logic after all scenario steps are finished.
    /// </summary>
    public interface IScenarioTearDown
    {
        /// <summary>
        /// Method executed for each scenario defined on the feature fixture class.
        /// The method is executed from within the scenario method, just after all scenario steps, but still within the context of scenario decorators.
        /// The method is called regardless of step execution results.
        /// </summary>
        Task OnScenarioTearDown();
    }
}