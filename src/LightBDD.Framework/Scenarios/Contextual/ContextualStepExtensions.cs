using System;
using System.Diagnostics;
using LightBDD.Framework.Scenarios.Contextual.Implementation;

namespace LightBDD.Framework.Scenarios.Contextual
{
    /// <summary>
    /// Extensions allowing to create contextual <see cref="ICompositeStepBuilder"/> instance.
    /// </summary>
    [DebuggerStepThrough]
    public static class ContextualStepExtensions
    {
        /// <summary>
        /// Specifies that scenario will be executed in dedicated context of <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.
        /// The context instance will be created by calling default constructor just before scenario execution.
        /// 
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by scenario runner. By default is it set to <c>true</c>.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="ICompositeStepBuilder"/> instance.</param>
        /// <param name="contextFactory">Context factory function.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Contextual runner.</returns>
        public static ICompositeStepBuilder<TContext> WithContext<TContext>(this ICompositeStepBuilder runner, Func<TContext> contextFactory, bool takeOwnership = true)
        {
            return new ContextualCompositeStepBuilder<TContext>(runner, () => contextFactory(), takeOwnership);
        }

        /// <summary>
        /// Specifies that scenario will be executed in dedicated <paramref name="context"/> of <typeparamref name="TContext"/> type.
        /// 
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by scenario runner. By default is it set to <c>false</c>.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="ICompositeStepBuilder"/> instance.</param>
        /// <param name="context">Context instance.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Contextual runner.</returns>
        public static ICompositeStepBuilder<TContext> WithContext<TContext>(this ICompositeStepBuilder runner, TContext context, bool takeOwnership = false)
        {
            return new ContextualCompositeStepBuilder<TContext>(runner, () => context, takeOwnership);
        }

        /// <summary>
        /// Returns runner that will be executing scenarios in dedicated context of <typeparamref name="TContext"/> type.<br/>
        /// The context instance will be created by calling default constructor just before scenario execution.
        /// 
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after scenario.
        /// </summary>
        /// <param name="runner"><see cref="ICompositeStepBuilder"/> instance.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static ICompositeStepBuilder<TContext> WithContext<TContext>(this ICompositeStepBuilder runner) where TContext : new()
        {
            return new ContextualCompositeStepBuilder<TContext>(runner, () => new TContext(), true);
        }
    }
}