using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Metadata.Implementation
{
    internal class StepInfo : IStepInfo
    {
        public StepInfo(Func<object, object[], Task> stepMethod, IStepNameInfo name)
        {
            StepMethod = stepMethod;
            Name = name;
        }

        public Func<object, object[], Task> StepMethod { get; private set; }
        public IStepNameInfo Name { get; private set; }
    }
}