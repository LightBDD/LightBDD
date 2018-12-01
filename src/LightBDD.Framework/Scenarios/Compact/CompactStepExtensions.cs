using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Compact.Implementation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Compact
{
    /// <summary>
    /// Extensions class allowing to use compact steps for defining LightBDD test steps.
    /// </summary>
    [DebuggerStepThrough]
    public static class CompactStepExtensions
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
    }
}