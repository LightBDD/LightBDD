using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Contextual.Implementation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Contextual
{
    /// <summary>
    /// Extensions allowing to create contextual <see cref="IBddRunner"/> instance.
    /// </summary>
    [DebuggerStepThrough]
    public static class ContextualScenarioExtensions
    {
        /// <summary>
        /// Specifies that scenario will be executed in dedicated context of <typeparamref name="TContext"/> type, created by <paramref name="contextFactory"/> function.
        /// The context instance will be created by calling default constructor just before scenario execution.
        /// 
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by scenario runner. By default is it set to <c>true</c>.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <param name="contextFactory">Context factory function.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, Func<TContext> contextFactory, bool takeOwnership = true)
        {
            return new ContextualBddRunner<TContext>(runner.Integrate())
                .Configure(cfg => cfg.WithContext(() => contextFactory(), takeOwnership));
        }

        /// <summary>
        /// Specifies that scenario will be executed in dedicated <paramref name="context"/> of <typeparamref name="TContext"/> type.
        /// 
        /// The <paramref name="takeOwnership"/> specifies if created context should be disposed (when implements <see cref="IDisposable"/> interface) by scenario runner. By default is it set to <c>false</c>.
        /// </summary>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <param name="context">Context instance.</param>
        /// <param name="takeOwnership">Specifies if scenario runner should take ownership of the context instance. If set to true and context instance implements <see cref="IDisposable"/>, it will be disposed after scenario finish.</param>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, TContext context, bool takeOwnership = false)
        {
            return new ContextualBddRunner<TContext>(runner.Integrate())
                .Configure(cfg => cfg.WithContext(() => context, takeOwnership));
        }

        /// <summary>
        /// Returns runner that will be executing scenarios in dedicated context of <typeparamref name="TContext"/> type.<br/>
        /// The context instance will be created by calling default constructor just before scenario execution.
        /// 
        /// All context instances implementing <see cref="IDisposable"/> interface will be disposed after scenario.
        /// </summary>
        /// <param name="runner"><see cref="IBddRunner"/> instance.</param>
        /// <typeparam name="TContext">Context type.</typeparam>
        /// <returns>Contextual runner.</returns>
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner)
        {
            return new ContextualBddRunner<TContext>(runner.Integrate())
                .Configure(cfg => cfg.WithContext(resolver => resolver.Resolve(typeof(TContext))));
        }

        public static IBddRunner<TContext> WithSetup<TContext>(this IBddRunner<TContext> runner, Action<TContext> onSetup)
        {
            return runner.ToContextual().Configure(x => x.WithSetup(onSetup.AsGeneric()));
        }

        public static IBddRunner<TContext> WithSetup<TContext>(this IBddRunner<TContext> runner, Func<TContext, Task> onSetup)
        {
            return runner.ToContextual().Configure(x => x.WithSetup(onSetup.AsGeneric()));
        }

        public static IBddRunner<TContext> WithTearDown<TContext>(this IBddRunner<TContext> runner, Action<TContext> onTearDown)
        {
            return runner.ToContextual().Configure(x => x.WithTearDown(onTearDown.AsGeneric()));
        }

        public static IBddRunner<TContext> WithTearDown<TContext>(this IBddRunner<TContext> runner, Func<TContext, Task> onTearDown)
        {
            return runner.ToContextual().Configure(x => x.WithTearDown(onTearDown.AsGeneric()));
        }

        private static ContextualBddRunner<TContext> ToContextual<TContext>(this IBddRunner<TContext> runner)
        {
            return runner as ContextualBddRunner<TContext> ?? new ContextualBddRunner<TContext>(runner.Integrate());
        }

        private static Func<object, Task> AsGeneric<T>(this Action<T> action)
        {
            return new AsGenericInvoker<T>(action).Invoke;
        }

        private static Func<object, Task> AsGeneric<T>(this Func<T,Task> action)
        {
            return new AsGenericAsyncInvoker<T>(action).Invoke;
        }

        [DebuggerStepThrough]
        private class AsGenericInvoker<T>
        {
            private readonly Action<T> _action;

            public AsGenericInvoker(Action<T> action)
            {
                _action = action;
            }

            public Task Invoke(object arg)
            {
                _action((T)arg);
                return Task.FromResult(0);
            }
        }

        [DebuggerStepThrough]
        private class AsGenericAsyncInvoker<T>
        {
            private readonly Func<T,Task> _action;

            public AsGenericAsyncInvoker(Func<T,Task> action)
            {
                _action = action;
            }

            public Task Invoke(object arg)
            {
                return _action((T)arg);
            }
        }
    }
}
