using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LightBDD.Core.Extensibility.Results
{
    /// <summary>
    /// Step result descriptor class representing composite step, i.e. one made of collection of sub-steps.
    /// It allows to define a list of additional sub steps that would be included in step execution, making given step passing only if all are successful.
    /// It allows also to specify dedicated context instance that would be passed to all sub-steps.
    /// </summary>
    [DebuggerStepThrough]
    public class CompositeStepResultDescriptor : IStepResultDescriptor
    {
        /// <summary>
        /// Constructor allowing to initialize instance with sub steps and context provider.
        /// </summary>
        /// <param name="subStepsContextProvider">Function providing context for all sub-steps.</param>
        /// <param name="subSteps">Sub steps.</param>
        public CompositeStepResultDescriptor(Func<object> subStepsContextProvider, IEnumerable<StepDescriptor> subSteps)
        {
            SubStepsContextProvider = subStepsContextProvider ?? throw new ArgumentNullException(nameof(subStepsContextProvider));
            SubSteps = subSteps;
        }

        /// <summary>
        /// Collection of sub steps.
        /// All defined sub steps would be executed just after descriptor is returned and will be included into overall step status as well as execution time.
        /// </summary>
        public IEnumerable<StepDescriptor> SubSteps { get; }

        /// <summary>
        /// Function providing instance of context for executing sub-steps.
        /// </summary>
        public Func<object> SubStepsContextProvider { get; }
    }
}