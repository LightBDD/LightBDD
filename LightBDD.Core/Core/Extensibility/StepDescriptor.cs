using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Extensibility
{
    public class StepDescriptor
    {
        public StepDescriptor(string predefinedStepType, string rawName, Func<object, object[], Task> stepInvocation, params ParameterDescriptor[] parameters)
        {
            RawName = rawName;
            StepInvocation = stepInvocation;
            Parameters = parameters;
            PredefinedStepType = predefinedStepType;
        }

        public StepDescriptor(string rawName, Func<object, object[], Task> stepInvocation, params ParameterDescriptor[] parameterDescriptors)
            : this(null, rawName, stepInvocation, parameterDescriptors)
        {
        }

        public string RawName { get; private set; }
        public string PredefinedStepType { get; private set; }
        public Func<object, object[], Task> StepInvocation { get; private set; }
        public ParameterDescriptor[] Parameters { get; private set; }
    }
}