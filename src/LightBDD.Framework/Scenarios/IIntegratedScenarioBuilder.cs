using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Interface offering access to core version of scenario builder.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IIntegratedScenarioBuilder<TContext> : IScenarioRunner<TContext>
    {
        /// <summary>
        /// Returns core version of scenario builder.
        /// </summary>
        ICoreScenarioStepsRunner Core { get; }
    }
}