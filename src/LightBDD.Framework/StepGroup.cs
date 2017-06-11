using System;
using System.Collections.Generic;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework
{
    //TODO: check naming and if further changes on StepResultDescriptor won't be breaking for StepGroup
    public class StepGroup : CompositeStepResultDescriptor
    {
        internal StepGroup(Func<object> contextProvider, IEnumerable<StepDescriptor> steps)
        : base(contextProvider, steps) { }

        public static IStepGroupBuilder DefineNew() => new StepGroupBuilder();
    }
}
