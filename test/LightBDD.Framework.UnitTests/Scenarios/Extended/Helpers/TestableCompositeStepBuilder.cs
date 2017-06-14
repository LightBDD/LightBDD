using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    class TestableCompositeStepBuilder : ICompositeStepBuilder, IIntegrableCompositeStepBuilder
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

        public CompositeStep Build() => _internal.Build();

        public IIntegrableCompositeStepBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            _internal.Integrate().AddSteps(steps);
            return this;
        }

        public IIntegrableCompositeStepBuilder WithStepContext(Func<object> contextProvider)
        {
            _internal.Integrate().WithStepContext(contextProvider);
            return this;
        }

        public TStepGroupBuilder Enrich<TStepGroupBuilder>(
            Func<IIntegrableCompositeStepBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
            => builderFactory(_internal.Integrate(), _configuration);
    }
}