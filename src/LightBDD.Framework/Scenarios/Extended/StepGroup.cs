using System.Collections.Generic;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Extended
{
    //TODO: this would easily break backward compat
    public class StepGroup : StepResultDescriptor
    {
        public StepGroup(IEnumerable<StepDescriptor> subSteps) : base(subSteps)
        {
        }
    }
}