using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios.Basic.Implementation;

namespace LightBDD.Framework.Scenarios.Basic
{
    [DebuggerStepThrough]
    public static class BasicStepExtensions
    {
        public static IStepGroupBuilder<NoContext> AddSteps(this IStepGroupBuilder<NoContext> builder, params Action[] steps)
        {
            builder.Integrate().AddSteps(steps.Select(BasicStepCompiler.ToSynchronousStep));
            return builder;
        }

        public static IStepGroupBuilder<NoContext> AddSteps(this IStepGroupBuilder<NoContext> builder, params Func<Task>[] steps)
        {
            builder.Integrate().AddSteps(steps.Select(BasicStepCompiler.ToAsynchronousStep));
            return builder;
        }
    }
}