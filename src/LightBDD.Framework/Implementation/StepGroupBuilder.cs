using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Extensibility;

namespace LightBDD.Framework.Implementation
{
    internal class StepGroupBuilder : LightBddConfigurationAware, IStepGroupBuilder, IIntegrableStepGroupBuilder
    {
        private readonly List<StepDescriptor> _steps = new List<StepDescriptor>();

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
            return builderFactory(this, Configuration);
        }
    }
}