using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Extensibility.Execution
{
    /// <summary>
    /// Base attribute allowing to enhance scenario execution with additional logic.
    /// The extensions would be executed in order specified by <see cref="Order"/> property, after globally registered extensions with <see cref="ExecutionExtensionsConfiguration"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ScenarioExecutionExtensionAttribute : Attribute, IScenarioExecutionExtension
    {
        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Method allowing to decorate scenario invocation specified by <paramref name="scenarioInvocation"/>.
        /// </summary>
        /// <param name="scenario">Scenario that is being executed.</param>
        /// <param name="scenarioInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        public abstract Task ExecuteAsync(IScenarioInfo scenario, Func<Task> scenarioInvocation);
    }
}