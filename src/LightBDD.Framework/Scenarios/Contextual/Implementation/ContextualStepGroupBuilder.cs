using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualStepGroupBuilder<TContext> : IStepGroupBuilder<TContext>, IIntegrableStepGroupBuilder
    {
        private readonly IIntegrableStepGroupBuilder _target;

        public ContextualStepGroupBuilder(IStepGroupBuilder runner, Func<object> contextProvider)
        {
            _target = runner.Integrate().WithStepContext(contextProvider);
        }

        public StepGroup Build()
        {
            return _target.Build();
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            _target.AddSteps(steps);
            return this;
        }

        public IIntegrableStepGroupBuilder WithStepContext(Func<object> contextProvider)
        {
            _target.WithStepContext(contextProvider);
            return this;
        }

        public TStepGroupBuilder Enrich<TStepGroupBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
            => _target.Enrich(new ContextualStepGroupBuilderEnricher<TStepGroupBuilder>(this, builderFactory).Enrich);

        [DebuggerStepThrough]
        private struct ContextualStepGroupBuilderEnricher<TStepGroupBuilder>
        {
            private readonly IIntegrableStepGroupBuilder _builder;
            private readonly Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> _builderFactory;

            public ContextualStepGroupBuilderEnricher(IIntegrableStepGroupBuilder builder, Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
            {
                _builder = builder;
                _builderFactory = builderFactory;
            }

            public TStepGroupBuilder Enrich(IIntegrableStepGroupBuilder _, LightBddConfiguration ctx)
            {
                return _builderFactory(_builder, ctx);
            }
        }
    }
}