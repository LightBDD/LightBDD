using System;
using System.Threading.Tasks;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility
{
    public class StepDescriptor
    {
        public StepDescriptor(string predefinedStepType, string rawName, Func<object, object[], Task> stepInvocation, params IParameterInfo[] parameterDescriptors)
        {
            RawName = rawName;
            StepInvocation = stepInvocation;
            ParameterDescriptors = parameterDescriptors;
            PredefinedStepType = predefinedStepType;
        }

        public StepDescriptor(string rawName, Func<object, object[], Task> stepInvocation, params IParameterInfo[] parameterDescriptors)
            : this(null, rawName, stepInvocation, parameterDescriptors)
        {
        }

        public string RawName { get; private set; }
        public string PredefinedStepType { get; private set; }
        public Func<object, object[], Task> StepInvocation { get; private set; }
        public IParameterInfo[] ParameterDescriptors { get; private set; }
    }
}