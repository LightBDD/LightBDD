using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface providing <seealso cref="OnScenarioSetUp"/> method to execute before run of each scenario defined on the feature fixture class that implements this interface.
    /// </summary>
    public interface IScenarioSetUp
    {
        /// <summary>
        /// Method executed for each scenario defined on the feature fixture class. The method is executed from within the scenario method, after all scenario level decorators are executed but before any scenario steps execution.
        /// </summary>
        Task OnScenarioSetUp();
    }
}