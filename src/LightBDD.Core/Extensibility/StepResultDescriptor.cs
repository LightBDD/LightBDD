using System.Collections.Generic;
using System.Linq;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Step result descriptor class that core engine expects to be returned after step execution.
    /// It allows to define a list of additional sub steps that would included in step execution, making given step passing only if all are successful.
    /// For regular steps, it is expected that <see cref="Default"/> value is returned.
    /// </summary>
    public class StepResultDescriptor
    {
        /// <summary>
        /// An instance describing regular step result, containing no additional sub steps.
        /// </summary>
        public static readonly StepResultDescriptor Default = new StepResultDescriptor(Enumerable.Empty<StepDescriptor>());

        /// <summary>
        /// Constructor allowing to initialize instance with sub steps definition.
        /// </summary>
        /// <param name="subSteps">Sub steps.</param>
        public StepResultDescriptor(IEnumerable<StepDescriptor> subSteps)
        {
            SubSteps = subSteps.ToArray();
        }

        /// <summary>
        /// Collection of sub steps.
        /// All difined sub steps would be executed just after descriptor is returned and will be included into overall step status as well as execution time.
        /// </summary>
        public IEnumerable<StepDescriptor> SubSteps { get; }
    }
}