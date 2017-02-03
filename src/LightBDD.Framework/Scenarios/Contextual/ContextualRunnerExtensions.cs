using System;
using System.Diagnostics;
using LightBDD.Framework.Scenarios.Contextual.Implementation;

namespace LightBDD.Framework.Scenarios.Contextual
{
    /// <summary>
    /// Extensions allowing to create contextual <see cref="IBddRunner"/> instance.
    /// </summary>
    [DebuggerStepThrough]
    public static class ContextualRunnerExtensions
    {
        /// <summary>
        /// Specifies that scenario would be executed in dedicated context of <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.
        /// The context instance would be created by calling default constructor just before scenario execution.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <param name="contextFactory">Context factory function.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, Func<TContext> contextFactory)
        {
            return new ContextualBddRunner<TContext>(runner, () => contextFactory());
        }

        /// <summary>
        /// Specifies that scenario would be executed in dedicated <paramref name="context"/> of <typeparamref name="TContext"/> type.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <param name="context">Context instance.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, TContext context)
        {
            return new ContextualBddRunner<TContext>(runner, () => context);
        }

        /// <summary>
        /// Returns runner that would be executing scenarios in dedicated context of <typeparamref name="TContext"/> type.<br/>
        /// The context instance would be created by calling default constructor just before scenario execution.
        /// </summary>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner) where TContext : new()
        {
            return new ContextualBddRunner<TContext>(runner, () => new TContext());
        }
    }
}
