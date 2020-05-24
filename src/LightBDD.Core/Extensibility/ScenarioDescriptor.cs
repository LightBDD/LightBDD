using LightBDD.Core.Internals;
using LightBDD.Core.Metadata;
using System;
using System.Reflection;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Class describing scenario including the scenario method info and parameters.
    /// It is designed to provide all required information for <see cref="CoreMetadataProvider"/>() method to build <see cref="IScenarioInfo"/> object.
    /// </summary>
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
                return Array.Empty<ParameterDescriptor>();

            var parameters = methodInfo.GetParameters();

            if (parameters.Length != arguments.Length)
                throw new InvalidOperationException($"Provided method {methodInfo} has different number of parameters than provided argument list: [{string.Join(", ", arguments)}]");

            var results = new ParameterDescriptor[parameters.Length];
            for (var i = 0; i < parameters.Length; ++i)
            {
                var parameterType = parameters[i].ParameterType.GetTypeInfo();
                var argument = arguments[i];
                var argumentType = argument?.GetType().GetTypeInfo();
                if (IsNullAssignmentToStruct(argument, parameterType) || IsArgumentTypeNotCompatible(argumentType, parameterType))
                {
                    var argumentDescription = argumentType != null
                        ? $"{argumentType} '{argument}'"
                        : "<null>";

                    throw new InvalidOperationException($"Provided argument {argumentDescription} is not assignable to parameter index {i} of method {methodInfo}");
                }
                results[i] = ParameterDescriptor.FromConstant(parameters[i], argument);
            }
            return results;
        }

        private static bool IsArgumentTypeNotCompatible(TypeInfo argumentType, TypeInfo parameterType)
        {
            return argumentType != null && !parameterType.IsAssignableFrom(argumentType);
        }

        private static bool IsNullAssignmentToStruct(object argument, TypeInfo parameterType)
        {
            return argument == null && parameterType.IsValueType && Nullable.GetUnderlyingType(parameterType.AsType()) == null;
        }
    }
}