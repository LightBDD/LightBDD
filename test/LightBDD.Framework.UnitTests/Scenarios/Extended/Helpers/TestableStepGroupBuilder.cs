using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.UnitTests.Scenarios.Extended.Helpers
{
    class TestableStepGroupBuilder : IStepGroupBuilder, IIntegrableStepGroupBuilder
    {
        private readonly IStepGroupBuilder _internal = StepGroup.DefineNew();
        private readonly LightBddConfiguration _configuration;

        public TestableStepGroupBuilder() : this(new LightBddConfiguration())
        {
        }
        public TestableStepGroupBuilder(LightBddConfiguration configuration)
        {
            _configuration = configuration;
        }

        public StepGroup Build() => _internal.Build();

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            _internal.Integrate().AddSteps(steps);
            return this;
        }

        public IIntegrableStepGroupBuilder WithStepContext(Func<object> contextProvider)
        {
            _internal.Integrate().WithStepContext(contextProvider);
            return this;
        }

        public TStepGroupBuilder Enrich<TStepGroupBuilder>(
            Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
            => builderFactory(_internal.Integrate(), _configuration);
    }
}