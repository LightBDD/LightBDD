using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Class describing scenario step, including its metadata information as well invocation method together with parameters required to its execution.
    /// It is designed to provide all required information for <see cref="IMetadataProvider.GetStepName"/>() method to provide <see cref="IStepNameInfo"/> object.
    /// </summary>
    [DebuggerStepThrough]
    public class StepDescriptor
    {
        /// <summary>
        /// Constructor allowing to specify predefined step type, name, step invocation function and step parameters.
        /// </summary>
        /// <param name="predefinedStepType">Predefined step type - it can be <c>null</c>.</param>
        /// <param name="rawName">Step raw name.</param>
        /// <param name="stepInvocation">Step invocation function.</param>
        /// <param name="parameters">Step invocation function parameters.</param>
        /// <exception cref="ArgumentException">Throws when <paramref name="rawName"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="stepInvocation"/> or <paramref name="parameters"/> is null.</exception>
        public StepDescriptor(string predefinedStepType, string rawName, Func<object, object[], Task> stepInvocation, params ParameterDescriptor[] parameters)
        {
            if (string.IsNullOrWhiteSpace(rawName))
                throw new ArgumentException("Null or just white space is not allowed", nameof(rawName));
            if (stepInvocation == null)
                throw new ArgumentNullException(nameof(stepInvocation));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            RawName = rawName;
            StepInvocation = stepInvocation;
            Parameters = parameters;
            PredefinedStepType = predefinedStepType;
        }

        /// <summary>
        /// Constructor allowing to specify name, step invocation function and step parameters.
        /// The <see cref="PredefinedStepType"/> is set to <c>null</c>.
        /// </summary>
        /// <param name="rawName">Step raw name.</param>
        /// <param name="stepInvocation">Step invocation function.</param>
        /// <param name="parameters">Step invocation function parameters.</param>
        /// <exception cref="ArgumentException">Throws when <paramref name="rawName"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="stepInvocation"/> or <paramref name="parameters"/> is null.</exception>
        public StepDescriptor(string rawName, Func<object, object[], Task> stepInvocation, params ParameterDescriptor[] parameters)
            : this(null, rawName, stepInvocation, parameters)
        {
        }
        /// <summary>
        /// Returns step raw name.
        /// </summary>
        public string RawName { get; }
        /// <summary>
        /// Returns predefined step type.
        /// </summary>
        public string PredefinedStepType { get; }
        /// <summary>
        /// Returns step invocation function accepting scenario context object configured with <see cref="IScenarioRunner.WithContext"/>() method and step parameters.
        /// </summary>
        public Func<object, object[], Task> StepInvocation { get; }
        /// <summary>
        /// Returns step parameter descriptors.
        /// </summary>
        public ParameterDescriptor[] Parameters { get; }
    }
}