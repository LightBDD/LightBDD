using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualCompositeStepBuilder<TContext> : ICompositeStepBuilder<TContext>, IIntegrableStepGroupBuilder
    {
        private readonly IIntegrableStepGroupBuilder _target;

        public ContextualCompositeStepBuilder(ICompositeStepBuilder runner, Func<object> contextProvider)
        {
            _target = runner.Integrate().WithStepContext(contextProvider);
        }

        public CompositeStep Build()
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

        public TEnrichedBuilder Enrich<TEnrichedBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> builderFactory)
            => _target.Enrich(new ContextualCompositeStepBuilderEnricher<TEnrichedBuilder>(this, builderFactory).Enrich);

        [DebuggerStepThrough]
        private struct ContextualCompositeStepBuilderEnricher<TEnrichedBuilder>
        {
            private readonly IIntegrableStepGroupBuilder _builder;
            private readonly Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> _builderFactory;

            public ContextualCompositeStepBuilderEnricher(IIntegrableStepGroupBuilder builder, Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> builderFactory)
            {
                _builder = builder;
                _builderFactory = builderFactory;
            }

            public TEnrichedBuilder Enrich(IIntegrableStepGroupBuilder _, LightBddConfiguration ctx)
            {
                return _builderFactory(_builder, ctx);
            }
        }
    }
}