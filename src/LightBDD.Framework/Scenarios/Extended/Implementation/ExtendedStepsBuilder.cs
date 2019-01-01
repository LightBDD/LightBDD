using LightBDD.Framework.Extensibility;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LightBDD.Framework.Scenarios.Extended.Implementation
{
    [DebuggerStepThrough]
    internal class ExtendedStepsBuilder<TBuilder, TContext> where TBuilder : IStepGroupBuilder<TContext>
    {
        private readonly TBuilder _builder;
        private readonly ExtendedStepCompiler<TContext> _stepCompiler;

        public ExtendedStepsBuilder(TBuilder builder)
        {
            _builder = builder;
            _stepCompiler = new ExtendedStepCompiler<TContext>(builder.Integrate().Configuration);
        }

        public TBuilder AddSteps(Expression<Func<TContext, Task>>[] steps)
        {
            _builder.Integrate().AddSteps(steps.Select(_stepCompiler.ToStep));
            return _builder;
        }

        public TBuilder AddSteps(Expression<Action<TContext>>[] steps)
        {
            _builder.Integrate().AddSteps(steps.Select(_stepCompiler.ToStep));
            return _builder;
        }
    }
}