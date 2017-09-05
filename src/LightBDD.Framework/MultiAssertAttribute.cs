using System;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Framework
{
    /// <summary>
    /// Attribute specifying that step method should report e
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MultiAssertAttribute : Attribute, IStepExecutionExtensionAttribute
    {
        public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            step.ConfigureExecutionAbortOnException(ex=>false);
            await stepInvocation();
        }

        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// </summary>
        public int Order { get; set; }
    }
}