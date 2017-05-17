using System.Collections.Generic;
using LightBDD.Core.Extensibility;

namespace LightBDD.Framework.Scenarios.Extended
{
    //TODO: this would easily break backward compat
    /// <summary>
    /// Class describing a step group. It should be used as a return value of step methods that consists of sub step group.
    /// </summary>
    public class StepGroup : StepResultDescriptor
    {
        internal StepGroup(IEnumerable<StepDescriptor> subSteps) : base(subSteps)
        {
        }
    }
}