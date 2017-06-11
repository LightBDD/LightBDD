using System;
using System.Diagnostics;
using LightBDD.Framework.Scenarios.Contextual.Implementation;

namespace LightBDD.Framework.Scenarios.Contextual
{
    /// <summary>
    /// Extensions allowing to create contextual <see cref="IStepGroupBuilder"/> instance.
    /// </summary>
    [DebuggerStepThrough]
    public static class ContextualStepExtensions
    {
        /// <summary>
        /// Specifies that scenario would be executed in dedicated context of <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.
        /// The context instance would be created by calling default constructor just before scenario execution.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IStepGroupBuilder"/> instance.</param>
        /// <param name="contextFactory">Context factory function.</param>
        /// <returns>Contextual runner.</returns>
        public static IStepGroupBuilder<TContext> WithContext<TContext>(this IStepGroupBuilder runner, Func<TContext> contextFactory)
        {
            return new ContextualStepGroupBuilder<TContext>(runner, () => contextFactory());
        }

        /// <summary>
        /// Specifies that scenario would be executed in dedicated <paramref name="context"/> of <typeparamref name="TContext"/> type.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IStepGroupBuilder"/> instance.</param>
        /// <param name="context">Context instance.</param>
        /// <returns>Contextual runner.</returns>
        public static IStepGroupBuilder<TContext> WithContext<TContext>(this IStepGroupBuilder runner, TContext context)
        {
            return new ContextualStepGroupBuilder<TContext>(runner, () => context);
        }

        /// <summary>
        /// Returns runner that would be executing scenarios in dedicated context of <typeparamref name="TContext"/> type.<br/>
        /// The context instance would be created by calling default constructor just before scenario execution.
        /// </summary>
        /// <param name="runner"><see cref="IStepGroupBuilder"/> instance.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static IStepGroupBuilder<TContext> WithContext<TContext>(this IStepGroupBuilder runner) where TContext : new()
        {
            return new ContextualStepGroupBuilder<TContext>(runner, () => new TContext());
        }
    }
}