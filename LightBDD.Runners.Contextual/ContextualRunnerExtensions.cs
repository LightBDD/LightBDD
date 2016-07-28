using System;

namespace LightBDD
{
    public static class ContextualRunnerExtensions
    {
        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, Func<TContext> contextFactory)
        {
            return new ContextualBddRunner<TContext>(runner, () => contextFactory());
        }

        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner, TContext context)
        {
            return new ContextualBddRunner<TContext>(runner, () => context);
        }

        public static IBddRunner<TContext> WithContext<TContext>(this IBddRunner runner) where TContext : new()
        {
            return new ContextualBddRunner<TContext>(runner, () => new TContext());
        }
    }
}
