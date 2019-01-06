using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Interface allowing to run scenario composed in fluent way.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IScenarioRunner<TContext> : IScenarioBuilder<TContext>
    {
        /// <summary>
        /// Runs test scenario by executing all specified steps in provided order.
        /// </summary>
        Task RunAsync();
    }
}