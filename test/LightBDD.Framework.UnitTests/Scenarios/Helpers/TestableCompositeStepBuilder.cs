using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.UnitTests.Scenarios.Helpers
{
    internal class TestableCompositeStepBuilder : ICompositeStepBuilder, IIntegrableCompositeStepBuilder
    {
        private readonly ICompositeStepBuilder _internal = CompositeStep.DefineNew();
        private readonly LightBddConfiguration _configuration;

        public TestableCompositeStepBuilder() : this(new LightBddConfiguration())
        {
        }
        public TestableCompositeStepBuilder(LightBddConfiguration configuration)
        {
            _configuration = configuration;
        }

        public CompositeStep Build()
        {
            return _internal.Build();
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            _internal.Integrate().AddSteps(steps);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<IDependencyResolver, object> contextProvider, Action<ContainerConfigurator> scopeConfigurator)
        {
            _internal.Integrate().WithStepContext(contextProvider, scopeConfigurator);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider, bool takeOwnership)
        {
            _internal.Integrate().WithStepContext(contextProvider, takeOwnership);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider)
        {
            _internal.Integrate().WithStepContext(contextProvider, false);
            return this;
        }

        public TStepGroupBuilder Enrich<TStepGroupBuilder>(
            Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
        {
            return builderFactory(_internal.Integrate(), _configuration);
        }
    }
}