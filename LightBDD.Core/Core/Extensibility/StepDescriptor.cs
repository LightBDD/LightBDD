using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility
{
    public class StepDescriptor
    {
        public StepDescriptor(string predefinedStepType, string rawName, Func<object, object[], Task> stepInvocation, params ParameterDescriptor[] parameters)
        {
            if (string.IsNullOrWhiteSpace(rawName))
                throw new ArgumentException("Null or just white space is not allowed", nameof(rawName));

            if (stepInvocation == null)
                throw new ArgumentNullException(nameof(stepInvocation));

            RawName = rawName;
            StepInvocation = stepInvocation;
            Parameters = parameters;
            PredefinedStepType = predefinedStepType;
        }

        public StepDescriptor(string rawName, Func<object, object[], Task> stepInvocation, params ParameterDescriptor[] parameterDescriptors)
            : this(null, rawName, stepInvocation, parameterDescriptors)
        {
        }

        public string RawName { get; }
        public string PredefinedStepType { get; }
        public Func<object, object[], Task> StepInvocation { get; }
        public ParameterDescriptor[] Parameters { get; }
    }
}