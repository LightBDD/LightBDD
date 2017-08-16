using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Execution;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Base attribute allowing to enhance step execution with additional logic.
    /// The extensions would be executed in order specified by <see cref="Order"/> property, after globally registered extensions with <see cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class StepExecutionExtensionAttribute : Attribute, IStepExecutionExtension
    {
        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Method allowing to decorate step invocation specified by <paramref name="stepInvocation"/>.
        /// </summary>
        /// <param name="step">Step that is being executed.</param>
        /// <param name="stepInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        public abstract Task ExecuteAsync(IStep step, Func<Task> stepInvocation);
    }
}