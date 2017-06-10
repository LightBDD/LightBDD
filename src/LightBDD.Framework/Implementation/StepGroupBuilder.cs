using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Implementation
{
    internal class StepGroupBuilder : IStepGroupBuilder<NoContext>, IIntegrableStepGroupBuilder
    {
        private readonly List<StepDescriptor> _steps = new List<StepDescriptor>();
        private readonly LightBddConfiguration _configuration;

        public StepGroupBuilder(LightBddConfiguration configuration)
        {
            _configuration = configuration;
        }

        public StepGroup Build()
        {
            return new StepGroup(_steps);
        }

        public void AddSteps(IEnumerable<StepDescriptor> steps)
        {
            _steps.AddRange(steps);
        }

        public TStepGroupBuilder Enrich<TStepGroupBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory)
        {
            return builderFactory(this, _configuration);
        }
    }
}