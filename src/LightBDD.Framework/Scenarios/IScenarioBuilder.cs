using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Interface allowing to compose scenario in fluent way.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IScenarioBuilder<TContext> : IStepGroupBuilder<TContext>
    {
        /// <summary>
        /// Runs test scenario by executing all specified steps.
        /// </summary>
        Task RunAsync();
    }
}