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

        public TestableCompositeStepBuilder() : this(new LightBddConfiguration())
        {
        }

        public TestableCompositeStepBuilder(LightBddConfiguration configuration)
        {
            Configuration = configuration;
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

        public LightBddConfiguration Configuration { get; }

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
    }
}