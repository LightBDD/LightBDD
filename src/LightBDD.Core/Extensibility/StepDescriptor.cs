using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Class describing scenario step, including its metadata information as well invocation method together with parameters required to its execution.
    /// It is designed to provide all required information for <see cref="CoreMetadataProvider.GetStepName"/>() method to provide <see cref="IStepNameInfo"/> object.
    /// </summary>
    public class StepDescriptor
    {
        /// <summary>
        /// Constructor allowing to specify name, step invocation function and step parameters.
        /// The <see cref="PredefinedStepType"/> is set to <c>null</c>.
        /// </summary>
        /// <param name="rawName">Step raw name.</param>
        /// <param name="stepInvocation">Step invocation function.</param>
        /// <param name="parameters">Step invocation function parameters.</param>
        /// <exception cref="ArgumentException">Throws when <paramref name="rawName"/> is null or empty.</exception>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="stepInvocation"/> or <paramref name="parameters"/> is null.</exception>
        public StepDescriptor(string rawName, StepFunc stepInvocation, params ParameterDescriptor[] parameters)
            : this(null, rawName, stepInvocation, parameters)
        {
        }

        /// <summary>
        /// Constructor allowing to specify predefined step type, methodInfo, step invocation function and step parameters.
        /// </summary>
        /// <param name="methodInfo">Step method info.</param>
        /// <param name="stepInvocation">Step invocation function.</param>
        /// <param name="parameters">Step invocation function parameters.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="methodInfo"/>, <paramref name="stepInvocation"/> or <paramref name="parameters"/> is null.</exception>
        public StepDescriptor(MethodBase methodInfo, StepFunc stepInvocation, params ParameterDescriptor[] parameters)
            : this(methodInfo ?? throw new ArgumentNullException(nameof(methodInfo)), methodInfo.Name, stepInvocation, parameters) { }

        /// <summary>
        /// Creates invalid descriptor indicating that original descriptor creation failed due to <paramref name="creationException"/> exception.
        /// Using this method will allow LightBDD to properly capture the invalid steps in the reports, helping with locating and correcting them properly.
        /// </summary>
        public static StepDescriptor CreateInvalid(Exception creationException) => new StepDescriptor(creationException);

        private StepDescriptor(MethodBase methodInfo, string rawName, StepFunc stepInvocation, params ParameterDescriptor[] parameters)
        {
            if (string.IsNullOrWhiteSpace(rawName))
                throw new ArgumentException("Null or just white space is not allowed", nameof(rawName));
            RawName = rawName;
            StepInvocation = stepInvocation ?? throw new ArgumentNullException(nameof(stepInvocation));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            MethodInfo = methodInfo;
        }

        private StepDescriptor(Exception creationException)
        {
            CreationException = creationException ?? throw new ArgumentNullException(nameof(creationException));
            RawName = "--INVALID STEP--";
            Parameters = Arrays<ParameterDescriptor>.Empty();
            StepInvocation = RunInvalidDescriptor;
        }

#pragma warning disable 1998
        private async Task<IStepResultDescriptor> RunInvalidDescriptor(object context, object[] args)
        {
            //TODO: LightBDD 4x simplify when NET45 dropped (Task.FromException())
            ExceptionDispatchInfo.Capture(CreationException).Throw();
            return null;
        }
#pragma warning restore 1998

        /// <summary>
        /// Returns step raw name.
        /// </summary>
        public string RawName { get; }

        /// <summary>
        /// Returns predefined step type.
        /// </summary>
        public string PredefinedStepType { get; set; }

        /// <summary>
        /// Returns method info describing the step or null if method info is not available.
        /// </summary>
        public MethodBase MethodInfo { get; }

        /// <summary>
        /// Returns step invocation function accepting scenario context object configured with <see cref="ICoreScenarioBuilder.WithContext(Func{object},bool)"/>() method and step parameters.
        /// </summary>
        public StepFunc StepInvocation { get; }

        /// <summary>
        /// Returns step parameter descriptors.
        /// </summary>
        public ParameterDescriptor[] Parameters { get; }

        /// <summary>
        /// Returns exception occured during descriptor creation or <c>null</c> if descriptor is valid.
        /// The value is set by <see cref="CreateInvalid"/> method.
        /// </summary>
        public Exception CreationException { get; }

        /// <summary>
        /// Returns true if descriptor is valid or false if descriptor was created by <see cref="CreateInvalid"/> method.
        /// </summary>
        public bool IsValid => CreationException == null;
    }
}