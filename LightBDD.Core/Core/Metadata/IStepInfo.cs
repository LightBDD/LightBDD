using System;
using System.Threading.Tasks;

namespace LightBDD.Core.Metadata
{
    public interface IStepInfo
    {
        Func<object, object[], Task> StepMethod { get; }
        IStepNameInfo Name { get; }
    }
}