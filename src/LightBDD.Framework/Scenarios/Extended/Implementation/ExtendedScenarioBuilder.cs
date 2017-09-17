using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Extended.Implementation
{
    [DebuggerStepThrough]
    internal class ExtendedScenarioBuilder<TContext>
    {
        private readonly IIntegrableStepGroupBuilder _builder;
        private readonly ExtendedStepCompiler<TContext> _stepCompiler;

        private ExtendedScenarioBuilder(IIntegrableStepGroupBuilder builder, LightBddConfiguration configuration)
        {
            _builder = builder;
            _stepCompiler = new ExtendedStepCompiler<TContext>(configuration);
        }

        public static ExtendedScenarioBuilder<TContext> Create(IIntegrableStepGroupBuilder builder, LightBddConfiguration configuration)
        {
            return new ExtendedScenarioBuilder<TContext>(builder, configuration);
        }

        public void AddSteps(Expression<Func<TContext, Task>>[] steps)
        {
            _builder.AddSteps(steps.Select(_stepCompiler.ToStep));
        }

        public void AddSteps(Expression<Action<TContext>>[] steps)
        {
            _builder.AddSteps(steps.Select(_stepCompiler.ToStep));
        }
    }
}