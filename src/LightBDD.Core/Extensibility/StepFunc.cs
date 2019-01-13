using System.Threading.Tasks;
using LightBDD.Core.Extensibility.Results;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Step function.
    /// </summary>
    public delegate Task<IStepResultDescriptor> StepFunc(object context, object[] args);
}