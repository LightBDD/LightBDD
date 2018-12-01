using LightBDD.Framework.Scenarios.Fluent.Implementation;
using System.Diagnostics;

namespace LightBDD.Framework.Scenarios.Fluent
{
    /// <summary>
    /// Extensions class allowing to define LightBDD scenarios in fluent way.
    /// </summary>
    [DebuggerStepThrough]
    public static class FluentScenarioExtensions
    {
        /// <summary>
        /// Allows to define and execute scenario in fluent way.<br/>
        /// Example usage:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-1")]
        /// public async Task My_scenario()
        /// {
        ///     await Runner.NewScenario()
        ///         .AddSteps( /* ... */ )
        ///         .RunAsync();
        /// }
        /// </code>
        /// Example usage with context:
        /// <code>
        /// [Scenario]
        /// [Label("Ticket-1")]
        /// public async Task My_scenario()
        /// {
        ///     await Runner.WithContext&lt;MyContext&gt;()
        ///         .NewScenario()
        ///         .AddSteps( /* ... */ )
        ///         .RunAsync();
        /// }
        /// </code>
        /// </summary>
        /// <param name="runner">Runner.</param>
        /// <returns><see cref="IScenarioBuilder{TContext}"/> instance.</returns>
        public static IScenarioBuilder<TContext> NewScenario<TContext>(this IBddRunner<TContext> runner)
        {
            return new ScenarioBuilder<TContext>(runner);
        }
    }
}
