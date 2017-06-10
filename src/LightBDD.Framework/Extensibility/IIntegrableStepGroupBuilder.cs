using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Extensibility
{
    public interface IIntegrableStepGroupBuilder
    {
        void AddSteps(IEnumerable<StepDescriptor> steps);
        TStepGroupBuilder Enrich<TStepGroupBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory);
    }
}