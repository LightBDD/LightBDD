#nullable enable
using System.Collections.Generic;
using LightBDD.Core.Dependencies;

namespace LightBDD.Core.Extensibility.Results
{
    /// <summary>
    /// Step result descriptor class representing composite step, i.e. one made of collection of sub-steps.
    /// It allows to define a list of additional sub steps that would be included in step execution, making given step passing only if all are successful.
    /// It allows also to specify dedicated context instance that would be passed to all sub-steps.
    /// </summary>
    public class CompositeStepResultDescriptor : IStepResultDescriptor
    {
        /// <summary>
        /// Constructor allowing to initialize instance with sub steps and context provider.
        /// </summary>
        /// <param name="subStepsContext">Context descriptor that will be used to instantiate context for the sub steps.</param>
        /// <param name="subSteps">Sub steps.</param>
        public CompositeStepResultDescriptor(Resolvable<object?> subStepsContext, IEnumerable<StepDescriptor> subSteps)
        {
            SubStepsContext = subStepsContext;
            SubSteps = subSteps;
        }

        /// <summary>
        /// Collection of sub steps.
        /// All defined sub steps would be executed just after descriptor is returned and will be included into overall step status as well as execution time.
        /// </summary>
        public IEnumerable<StepDescriptor> SubSteps { get; }

        /// <summary>
        /// Returns context provider that will be used to instantiate context for the sub steps.
        /// </summary>
        public Resolvable<object?> SubStepsContext { get; }
    }
}