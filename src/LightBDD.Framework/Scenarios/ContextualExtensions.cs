using System;
using LightBDD.Framework.Scenarios.Implementation;
using LightBDD.Core.Dependencies;

namespace LightBDD.Framework.Scenarios
{
    /// <summary>
    /// Extensions allowing to create contextual <see cref="IBddRunner"/> instance.
    /// </summary>
    public static class ContextualExtensions
    {
        /// <summary>
        /// Specifies that scenario will be executed in dedicated context of <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.<br/>
        /// The context instance will be created just before scenario execution.<br/>
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by scenario runner. By default is it set to <c>true</c>.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <param name="contextFactory">Context factory function.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, Func<TContext> contextFactory, bool takeOwnership = true)
        {
            return new ContextualBddRunner<TContext>(runner, () => contextFactory(), takeOwnership);
        }

        /// <summary>
        /// Specifies that scenario will be executed in dedicated context of <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.<br/>
        /// The <paramref name="onConfigure"/> function can be used to configure the context after instantiation.<br/>
        /// The context instance will be created just before scenario execution.<br/>
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after scenario execution.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <param name="contextFactory">Context factory function.</param>
        /// <param name="onConfigure">Custom context configuration executed after context creation. Can be null.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, Func<IDependencyResolver, TContext> contextFactory, Action<TContext> onConfigure = null)
        {
            return new ContextualBddRunner<TContext>(runner, resolver =>
            {
                var context = contextFactory.Invoke(resolver);
                onConfigure?.Invoke(context);
                return context;
            });
        }

        /// <summary>
        /// Specifies that scenario will be executed in dedicated <paramref name="context"/> of <typeparamref name="TContext"/> type.<br/>
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by scenario runner. By default is it set to <c>false</c>.<br/>
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <param name="context">Context instance.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, TContext context, bool takeOwnership = false)
        {
            return new ContextualBddRunner<TContext>(runner, () => context, takeOwnership);
        }

        /// <summary>
        /// Specifies that scenario will be executed in dedicated context of <typeparamref name="TContext"/> type.<br/>
        /// The context instance will be created by calling default constructor just before scenario execution.<br/>
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after scenario execution.
        /// </summary>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner)
        {
            return new ContextualBddRunner<TContext>(runner, resolver => resolver.Resolve(typeof(TContext)));
        }

        /// <summary>
        /// Specifies that scenario will be executed in dedicated context of <typeparamref name="TContext"/> type.<br/>
        /// The <paramref name="onConfigure"/> function can be used to configure the context after instantiation.<br/>
        /// The context instance will be created by calling default constructor just before scenario execution.<br/>
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after scenario execution.
        /// </summary>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="onConfigure">Custom context configuration executed after context creation. Can be null.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, Action<TContext> onConfigure)
        {
            return new ContextualBddRunner<TContext>(runner, resolver =>
            {
                var context = resolver.Resolve<TContext>();
                onConfigure?.Invoke(context);
                return context;
            });
        }

        /// <summary>
        /// Specifies that composite step will be executed in dedicated context of <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.<br/>
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by runner. By default is it set to <c>true</c>.
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
        /// Specifies that composite step will be executed in dedicated <paramref name="context"/> of <typeparamref name="TContext"/> type.<br/>
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by runner. By default is it set to <c>false</c>.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="ICompositeStepBuilder"/> instance.</param>
        /// <param name="context">Context instance.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after composite step finish.</param>
        /// <returns>Contextual runner.</returns>
        public static ICompositeStepBuilder<TContext> WithContext<TContext>(this ICompositeStepBuilder runner, TContext context, bool takeOwnership = false)
        {
            return new ContextualCompositeStepBuilder<TContext>(runner, () => context, takeOwnership);
        }

        /// <summary>
        /// Specifies that composite step will be executed in dedicated context <typeparamref name="TContext"/> type.<br/>
        /// The context instance will be created by calling default constructor just before composite step execution.<br/>
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after composite step execution.
        /// </summary>
        /// <param name="runner"><see cref="ICompositeStepBuilder"/> instance.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static ICompositeStepBuilder<TContext> WithContext<TContext>(this ICompositeStepBuilder runner)
        {
            return new ContextualCompositeStepBuilder<TContext>(runner, resolver => resolver.Resolve(typeof(TContext)));
        }

        /// <summary>
        /// Specifies that composite step will be executed in dedicated context <typeparamref name="TContext"/> type.<br/>
        /// The <paramref name="onConfigure"/> function can be used to configure the context after instantiation.<br/>
        /// The context instance will be created by calling default constructor just before composite step execution.<br/>
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after composite step execution.
        /// </summary>
        /// <param name="runner"><see cref="ICompositeStepBuilder"/> instance.</param>
        /// <param name="onConfigure">Custom context configuration executed after context creation. Can be null.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static ICompositeStepBuilder<TContext> WithContext<TContext>(this ICompositeStepBuilder runner, Action<TContext> onConfigure)
        {
            return new ContextualCompositeStepBuilder<TContext>(runner, resolver =>
            {
                var context = resolver.Resolve<TContext>();
                onConfigure?.Invoke(context);
                return context;
            });
        }

        /// <summary>
        /// Specifies that composite step will be executed in dedicated context <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.<br/>
        /// The <paramref name="onConfigure"/> function can be used to configure the context after instantiation.<br/>
        /// The context instance will be created by calling default constructor just before composite step execution.<br/>
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after composite step execution.
        /// </summary>
        /// <param name="runner"><see cref="ICompositeStepBuilder"/> instance.</param>
        /// <param name="contextFactory">Context factory function.</param>
        /// <param name="onConfigure">Custom context configuration executed after context creation. Can be null.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static ICompositeStepBuilder<TContext> WithContext<TContext>(this ICompositeStepBuilder runner, Func<IDependencyResolver, TContext> contextFactory, Action<TContext> onConfigure = null)
        {
            return new ContextualCompositeStepBuilder<TContext>(runner, resolver =>
            {
                var context = contextFactory.Invoke(resolver);
                onConfigure?.Invoke(context);
                return context;
            });
        }
    }
}
