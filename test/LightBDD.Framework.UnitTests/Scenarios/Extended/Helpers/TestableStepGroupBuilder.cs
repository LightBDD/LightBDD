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

        public void AddSteps(IEnumerable<StepDescriptor> steps) => StepGroupBuilderExtensions.Integrate<NoContext>(_internal).AddSteps(steps);

        public TStepGroupBuilder Enrich<TStepGroupBuilder>(
            Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
            => builderFactory(StepGroupBuilderExtensions.Integrate<NoContext>(_internal), _configuration);
    }
}