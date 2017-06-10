using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Extended.Implementation;

namespace LightBDD.Framework.Scenarios.Extended
{
    [DebuggerStepThrough]
    public static class ExtendedStepExtensions
    {
        public static IStepGroupBuilder<TContext> AddSteps<TContext>(this IStepGroupBuilder<TContext> builder, params Expression<Action<TContext>>[] steps)
        {
            AsExtended(builder).AddSteps(steps);
            return builder;
        }

        public static IStepGroupBuilder<TContext> AddSteps<TContext>(this IStepGroupBuilder<TContext> builder, params Expression<Func<TContext, Task>>[] steps)
        {
            AsExtended(builder).AddSteps(steps);
            return builder;
        }

        private static ExtendedStepGroupBuilder<TContext> AsExtended<TContext>(this IStepGroupBuilder<TContext> runner)
        {
            return runner.Integrate().Enrich(ExtendedStepGroupBuilder<TContext>.Create);
        }
    }
}
