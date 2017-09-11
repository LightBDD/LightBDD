using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Interface describing step execution extension that can be used by LightBDD to decorate step execution.
    /// </summary>
    public interface IStepDecorator
    {
        /// <summary>
        /// Method allowing to decorate step invocation specified by <paramref name="stepInvocation"/>.
        /// </summary>
        /// <param name="step">Step that is being executed.</param>
        /// <param name="stepInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        Task ExecuteAsync(IStep step, Func<Task> stepInvocation);
    }
}