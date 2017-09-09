using System;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface describing step execution extension that can be used by LightBDD to decorate step execution.
    /// </summary>
    [Obsolete("Use IStepExtension instead", true)]
    public interface IStepExecutionExtension : IStepExtension
    {
    }
}