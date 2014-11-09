using System.Collections.Generic;
using LightBDD.Naming;

namespace LightBDD.Results
{
    /// <summary>
    /// Interface describing step name.
    /// </summary>
    public interface IStepName
    {
        /// <summary>
        /// Returns step name format string where {n} parameter corresponds to nth element of ParameterValues.
        /// </summary>
        string NameFormat { get; }
        /// <summary>
        /// Parameter values used for this step.
        /// If step has been evaluated, ParameterValues would contain preformatted (with InvariantCulture) values of captured step parameters.
        /// If step has not been evaluated, ParameterValues may contain 
        /// </summary>
        IEnumerable<IStepParameter> Parameters { get; }
        /// <summary>
        /// Step type name or null if not present.
        /// </summary>
        string StepTypeName { get; }

        /// <summary>
        /// Formats full step name with given formatter.
        /// The returned name would contain StepTypeName followed by NameFormat with parameter values.
        /// </summary>
        string Format(IStepNameDecorator decorator);
    }
}