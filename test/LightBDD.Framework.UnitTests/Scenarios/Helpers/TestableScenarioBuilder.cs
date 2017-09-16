using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;
using LightBDD.Framework.Scenarios;

namespace LightBDD.Framework.UnitTests.Scenarios.Helpers
{
    public class TestableScenarioBuilder<T> : IScenarioBuilder<T>, IIntegrableStepGroupBuilder
    {
        public readonly List<StepDescriptor> Steps = new List<StepDescriptor>();
        public async Task RunAsync()
        {
            throw new NotImplementedException();
        }

        public IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps)
        {
            Steps.AddRange(steps);
            return this;
        }

        public IIntegrableStepGroupBuilder WithStepContext(Func<object> contextProvider)
        {
            throw new NotImplementedException();
        }

        public TEnrichedBuilder Enrich<TEnrichedBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TEnrichedBuilder> builderFactory)
        {
            return builderFactory(this, new LightBddConfiguration());
        }

        public CompositeStep Build()
        {
            throw new NotImplementedException();
        }
    }
}