using System;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Results;
using LightBDD.Core.Formatting;
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
        /// <exception cref="ArgumentException">Throws when <paramref name="rawName"/> is null or empty or contains control characters.</exception>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="stepInvocation"/> or <paramref name="parameters"/> is null.</exception>
        public StepDescriptor(string rawName, StepFunc stepInvocation, params ParameterDescriptor[] parameters)
             : this(null, VerifyRawName(rawName), stepInvocation, parameters)
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

        private StepDescriptor(MethodBase? methodInfo, string rawName, StepFunc stepInvocation, params ParameterDescriptor[] parameters)
        {
            RawName = rawName;
            StepInvocation = stepInvocation ?? throw new ArgumentNullException(nameof(stepInvocation));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            MethodInfo = methodInfo;
            IsNameFormattingRequired = methodInfo != null;
        }

        private StepDescriptor(Exception creationException)
        {
            CreationException = creationException ?? throw new ArgumentNullException(nameof(creationException));
            RawName = "<INVALID STEP>";
            Parameters = Array.Empty<ParameterDescriptor>();
            StepInvocation = RunInvalidDescriptor;
        }

        private static string VerifyRawName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
                throw new ArgumentException("Step name has to be specified and cannot contain only white characters.", nameof(rawName));
            for (var i = 0; i < rawName.Length; ++i)
            {
                if (char.IsControl(rawName[i]))
                    throw new ArgumentException($"Step name cannot contain control characters, got one at index {i} in '{rawName}'", nameof(rawName));
            }
            return rawName;
        }

        private Task<IStepResultDescriptor> RunInvalidDescriptor(object? context, object?[] args) => Task.FromException<IStepResultDescriptor>(CreationException!);

        /// <summary>
        /// Returns step raw name.
        /// </summary>
        public string RawName { get; }

        /// <summary>
        /// Returns or sets predefined step type. If null, the step type will be inferred from <seealso cref="RawName"/>.
        /// </summary>
        public string? PredefinedStepType { get; set; }

        /// <summary>
        /// Returns method info describing the step or null if method info is not available.
        /// </summary>
        public MethodBase? MethodInfo { get; }

        /// <summary>
        /// Returns step invocation function accepting scenario context object configured with <see cref="ICoreScenarioBuilder.WithContext(Func{object},bool)"/>() method and step parameters.
        /// </summary>
        public StepFunc StepInvocation { get; }

        /// <summary>
        /// Returns step parameter descriptors.
        /// </summary>
        public ParameterDescriptor[] Parameters { get; }

        /// <summary>
        /// Returns exception occurred during descriptor creation or <c>null</c> if descriptor is valid.
        /// The value is set by <see cref="CreateInvalid"/> method.
        /// </summary>
        public Exception? CreationException { get; }

        /// <summary>
        /// Returns true if descriptor is valid or false if descriptor was created by <see cref="CreateInvalid"/> method.
        /// </summary>
        public bool IsValid => CreationException == null;

        /// <summary>
        /// Specifies if <see cref="RawName"/> requires formatting with configured <see cref="INameFormatter"/>.
        /// By default, it is set to true if <see cref="MethodInfo"/> is provided, otherwise the default value is false.
        /// </summary>
        public bool IsNameFormattingRequired { get; set; }
    }
}