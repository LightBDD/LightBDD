using System;
using System.Collections.Generic;

namespace LightBDD.Core.Extensibility.Results
{
    /// <summary>
    /// Step result descriptor class that core engine expects to be returned after step execution.
    /// It allows to define a list of additional sub steps that would included in step execution, making given step passing only if all are successful.
    /// </summary>
    public class CompositeStepResultDescriptor : IStepResultDescriptor
    {
        /// <summary>
        /// Constructor allowing to initialize instance with sub steps definition.
        /// </summary>
        /// <param name="subStepsContextProvider"></param>
        /// <param name="subSteps">Sub steps.</param>
        public CompositeStepResultDescriptor(Func<object> subStepsContextProvider, IEnumerable<StepDescriptor> subSteps)
        {
            SubStepsContextProvider = subStepsContextProvider ?? throw new ArgumentNullException(nameof(subStepsContextProvider));
            SubSteps = subSteps;
        }

        /// <summary>
        /// Collection of sub steps.
        /// All difined sub steps would be executed just after descriptor is returned and will be included into overall step status as well as execution time.
        /// </summary>
        public IEnumerable<StepDescriptor> SubSteps { get; }

        public Func<object> SubStepsContextProvider { get; }
    }
}