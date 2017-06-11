using System;
using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Extensibility
{
    public interface IIntegrableStepGroupBuilder
    {
        IIntegrableStepGroupBuilder AddSteps(IEnumerable<StepDescriptor> steps);
        IIntegrableStepGroupBuilder WithStepContext(Func<object> contextProvider);
        TStepGroupBuilder Enrich<TStepGroupBuilder>(Func<IIntegrableStepGroupBuilder, LightBddConfiguration, TStepGroupBuilder> builderFactory);
        StepGroup Build();
    }
}