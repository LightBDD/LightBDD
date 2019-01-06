using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Framework.Scenarios.Implementation;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Extensions class allowing to use compact steps for defining LightBDD test steps.
    /// </summary>
    public static class CompactExtensions
    {
        /// <summary>
        /// Adds step with name specified by <paramref name="name"/> parameter and action specified by <paramref name="step"/> parameter to the composite step.
        /// </summary>
        /// <typeparam name="TContext">Scenario context, if specified.</typeparam>
        /// <param name="builder">Builder.</param>
        /// <param name="name">Step name to be rendered.</param>
        /// <param name="step">Step action</param>
        /// <returns>Builder.</returns>
        public static ICompositeStepBuilder<TContext> AddStep<TContext>(this ICompositeStepBuilder<TContext> builder, string name, Action<TContext> step)
        {
            builder.Integrate().AddSteps(new[] { CompactStepCompiler.ToSynchronousStep(name, step) });
            return builder;
        }

        /// <summary>
        /// Adds asynchronous step with name specified by <paramref name="name"/> parameter and action specified by <paramref name="step"/> parameter to the composite step.
        /// </summary>
        /// <typeparam name="TContext">Scenario context, if specified.</typeparam>
        /// <param name="builder">Builder.</param>
        /// <param name="name">Step name to be rendered.</param>
        /// <param name="step">Step action</param>
        /// <returns>Builder.</returns>
        public static ICompositeStepBuilder<TContext> AddAsyncStep<TContext>(this ICompositeStepBuilder<TContext> builder, string name, Func<TContext, Task> step)
        {
            builder.Integrate().AddSteps(new[] { CompactStepCompiler.ToAsynchronousStep(name, step) });
            return builder;
        }

        /// <summary>
        /// Adds step with name specified by <paramref name="name"/> parameter and action specified by <paramref name="step"/> parameter to the scenario.
        /// </summary>
        /// <typeparam name="TContext">Scenario context, if specified.</typeparam>
        /// <param name="builder">Builder.</param>
        /// <param name="name">Step name to be rendered.</param>
        /// <param name="step">Step action</param>
        /// <returns>Builder.</returns>
        public static IScenarioRunner<TContext> AddStep<TContext>(this IScenarioBuilder<TContext> builder, string name, Action<TContext> step)
        {
            var integration = builder.Integrate();
            integration.Core.AddSteps(new[] { CompactStepCompiler.ToSynchronousStep(name, step) });
            return integration;
        }

        /// <summary>
        /// Adds asynchronous step with name specified by <paramref name="name"/> parameter and action specified by <paramref name="step"/> parameter to the scenario.
        /// </summary>
        /// <typeparam name="TContext">Scenario context, if specified.</typeparam>
        /// <param name="builder">Builder.</param>
        /// <param name="name">Step name to be rendered.</param>
        /// <param name="step">Step action</param>
        /// <returns>Builder.</returns>
        public static IScenarioRunner<TContext> AddAsyncStep<TContext>(this IScenarioBuilder<TContext> builder, string name, Func<TContext, Task> step)
        {
            var integration = builder.Integrate();
            integration.Core.AddSteps(new[] { CompactStepCompiler.ToAsynchronousStep(name, step) });
            return integration;
        }
    }
}