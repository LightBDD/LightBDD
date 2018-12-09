using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Results;
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
        private static Func<object, object[], Task<IStepResultDescriptor>> TransformStepInvocation(Func<object, object[], Task> stepInvocation)
        {
            return async (ctx, args) =>
            {
                await stepInvocation(ctx, args);
                return DefaultStepResultDescriptor.Instance;
            };
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
        public StepDescriptor(string rawName, Func<object, object[], Task<IStepResultDescriptor>> stepInvocation, params ParameterDescriptor[] parameters)
            : this((MethodBase)null, rawName, stepInvocation, parameters)
        {
        }
        /// <summary>
        /// Constructor allowing to specify predefined step type, methodInfo, step invocation function and step parameters.
        /// </summary>
        /// <param name="methodInfo">Step method info.</param>
        /// <param name="stepInvocation">Step invocation function.</param>
        /// <param name="parameters">Step invocation function parameters.</param>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="methodInfo"/>, <paramref name="stepInvocation"/> or <paramref name="parameters"/> is null.</exception>
        public StepDescriptor(MethodBase methodInfo, Func<object, object[], Task<IStepResultDescriptor>> stepInvocation, params ParameterDescriptor[] parameters)
            : this(methodInfo ?? throw new ArgumentNullException(nameof(methodInfo)), methodInfo.Name, stepInvocation, parameters) { }

        private StepDescriptor(MethodBase methodInfo, string rawName, Func<object, object[], Task<IStepResultDescriptor>> stepInvocation, params ParameterDescriptor[] parameters)
        {
            if (string.IsNullOrWhiteSpace(rawName))
                throw new ArgumentException("Null or just white space is not allowed", nameof(rawName));
            RawName = rawName;
            StepInvocation = stepInvocation ?? throw new ArgumentNullException(nameof(stepInvocation));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            MethodInfo = methodInfo;
        }

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
        /// Returns step invocation function accepting scenario context object configured with <see cref="IScenarioRunner.WithContext(Func{object},bool)"/>() method and step parameters.
        /// </summary>
        public Func<object, object[], Task<IStepResultDescriptor>> StepInvocation { get; }
        /// <summary>
        /// Returns step parameter descriptors.
        /// </summary>
        public ParameterDescriptor[] Parameters { get; }
    }
}