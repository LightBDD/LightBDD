using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Scenarios.Contextual.Implementation
{
    [DebuggerStepThrough]
    internal class ContextualCompositeStepBuilder<TContext> : ICompositeStepBuilder<TContext>, IIntegrableCompositeStepBuilder
    {
        private readonly IIntegrableCompositeStepBuilder _target;

        public ContextualCompositeStepBuilder(ICompositeStepBuilder runner, Func<object> contextProvider, bool takeOwnership)
        {
            _target = runner.Integrate().WithStepContext(contextProvider, takeOwnership);
        }

        public ContextualCompositeStepBuilder(ICompositeStepBuilder runner, Func<IDependencyResolver, object> contextResolver)
        {
            _target = runner.Integrate().WithStepContext(contextResolver);
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

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider, bool takeOwnership)
        {
            _target.WithStepContext(contextProvider, takeOwnership);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider)
        {
            _target.WithStepContext(contextProvider, false);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator)
        {
            _target.WithStepContext(contextProvider, scopeConfigurator);
            return this;
        }

        public TEnrichedBuilder Enrich<TEnrichedBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> builderFactory)
        {
            return _target.Enrich(new ContextualCompositeStepBuilderEnricher<TEnrichedBuilder>(this, builderFactory)
                .Enrich);
        }

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