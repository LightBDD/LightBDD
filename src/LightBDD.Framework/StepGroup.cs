using System.Collections.Generic;
using LightBDD.Core.Configuration;
using LightBDD.Core.Extensibility;
using LightBDD.Framework.Implementation;

namespace LightBDD.Framework
{
    //TODO: check naming and if further changes on StepResultDescriptor won't be breaking for StepGroup
    public class StepGroup : StepResultDescriptor
    {
        internal StepGroup(IEnumerable<StepDescriptor> steps)
        : base(steps) { }

        //TODO: provide proper configuration
        public static IStepGroupBuilder<NoContext> DefineNew() => new StepGroupBuilder(new LightBddConfiguration());
    }
}
