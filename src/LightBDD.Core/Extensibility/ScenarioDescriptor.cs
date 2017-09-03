using System;
using System.Diagnostics;
using System.Reflection;
using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Class describing scenario including the scenario method info and parameters.
    /// It is designed to provide all required information for <see cref="IMetadataProvider"/>() method to build <see cref="IScenarioInfo"/> object.
    /// </summary>
    [DebuggerStepThrough]
    public class ScenarioDescriptor
    {
        /// <summary>
        /// Constructor creating <see cref="ScenarioDescriptor"/> instance.
        /// </summary>
        /// <param name="methodInfo">Scenario method info.</param>
        /// <param name="arguments">Scenario arguments. If scenario arguments are not known (unable to obtain), the value of <paramref name="arguments"/> should be null.</param>
        /// <exception cref="InvalidOperationException">Thrown if number or <paramref name="arguments"/> is not null and does not match number of <paramref name="methodInfo"/> parameters.</exception>
        public ScenarioDescriptor(MethodBase methodInfo, object[] arguments)
        {
            MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            Parameters = BuildParameters(methodInfo, arguments);

        }
        /// <summary>
        /// Returns scenario method.
        /// </summary>
        public MethodBase MethodInfo { get; }

        /// <summary>
        /// Returns scenario method parameter descriptors or empty array if parameters are not known or method is parameterless.
        /// </summary>
        public ParameterDescriptor[] Parameters { get; }

        private static ParameterDescriptor[] BuildParameters(MethodBase methodInfo, object[] arguments)
        {
            if (arguments == null)
                return Arrays<ParameterDescriptor>.Empty();

            var parameters = methodInfo.GetParameters();

            if (parameters.Length != arguments.Length)
                throw new InvalidOperationException($"Provided method {methodInfo} has different number of parameters than provided argument list: [{string.Join(", ", arguments)}]");

            var results = new ParameterDescriptor[parameters.Length];
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameterType = parameters[i].ParameterType.GetTypeInfo();
                var argument = arguments[i];
                var argumentType = argument?.GetType();
                if ((argument == null && parameterType.IsValueType) || (argument != null && !parameterType.IsAssignableFrom(argument.GetType().GetTypeInfo())))
                    throw new InvalidOperationException($"Provided argument {argumentType} '{argument ?? "null"}' is not assignable to parameter index {i} of method {methodInfo}");
                results[i] = ParameterDescriptor.FromConstant(parameters[i], argument);
            }
            return results;
        }
    }
}