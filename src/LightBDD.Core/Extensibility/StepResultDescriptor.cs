using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Extensibility
{
    public class StepResultDescriptor
    {
        public static readonly StepResultDescriptor None = new StepResultDescriptor(Enumerable.Empty<StepDescriptor>());

        public StepResultDescriptor(IEnumerable<StepDescriptor> subSteps)
        {
            SubSteps = subSteps.ToArray();
        }

        public StepDescriptor[] SubSteps { get; }
    }
}