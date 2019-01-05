using LightBDD.Core.Extensibility;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Interface allowing to compose scenario in fluent way.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IScenarioBuilder<TContext>
    {
        IIntegratedScenarioBuilder<TContext> Integrate();
    }

    public interface IScenarioRunner<TContext> : IScenarioBuilder<TContext>
    {
        /// <summary>
        /// Runs test scenario by executing all specified steps.
        /// </summary>
        Task RunAsync();
    }

    public interface IIntegratedScenarioBuilder<TContext> : IScenarioRunner<TContext>
    {
        ICoreScenarioBuilder Core { get; }
    }
}