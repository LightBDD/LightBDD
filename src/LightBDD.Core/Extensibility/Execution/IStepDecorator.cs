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
        /// Method allowing to decorate step invocation specified by <paramref name="stepInvocation"/>.<br/>
        /// Any exceptions thrown from <paramref name="stepInvocation"/> will be wrapped in <see cref="ScenarioExecutionException"/>, which means that if this method suppose to handle exceptions, it will have to use <see cref="Exception.InnerException"/> property of <see cref="ScenarioExecutionException"/>.
        /// It is allowed to throw any exception type from this method.
        /// </summary>
        /// <param name="step">Step that is being executed.</param>
        /// <param name="stepInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        Task ExecuteAsync(IStep step, Func<Task> stepInvocation);
    }
}