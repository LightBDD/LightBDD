using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Core.Execution;
using LightBDD.Core.Extensibility.Execution;

namespace LightBDD.Framework
{
    /// <summary>
    /// Multi Assert Attribute allows to configure scenario and/or step method to always execute all sub-steps no matter what is their outcome and report any exceptions after all sub-steps are executed.
    /// If any sub-step execution finish with failed status, an <see cref="AggregateException"/> will be thrown with exceptions of all failed sub-steps.
    /// If none sub-steps failed but there are some with ignored status, the exception of first ignored sub-step would be thrown in order to properly ignore test in underlying test framework.
    /// All exceptions that have occurred during sub-steps execution would be included in the LightBDD report.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class MultiAssertAttribute : Attribute, IStepDecoratorAttribute, IScenarioDecoratorAttribute
    {
        /// <summary>
        /// Method allowing to decorate step invocation specified by <paramref name="stepInvocation"/>.
        /// </summary>
        /// <param name="step">Step that is being executed.</param>
        /// <param name="stepInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        public async Task ExecuteAsync(IStep step, Func<Task> stepInvocation)
        {
            step.ConfigureExecutionAbortOnSubStepException(ShouldAbortExecution);
            await stepInvocation();
        }

        /// <summary>
        /// Method allowing to decorate scenario invocation specified by <paramref name="scenarioInvocation"/>.
        /// </summary>
        /// <param name="scenario">Scenario that is being executed.</param>
        /// <param name="scenarioInvocation">Invocation that should be called in the method body.</param>
        /// <returns>Execution task.</returns>
        public async Task ExecuteAsync(IScenario scenario, Func<Task> scenarioInvocation)
        {
            scenario.ConfigureExecutionAbortOnSubStepException(ShouldAbortExecution);
            await scenarioInvocation();
        }

        private static bool ShouldAbortExecution(Exception exception)
        {
            return false;
        }

        /// <summary>
        /// Order in which extensions should be applied, where instances of lower values would be executed first.
        /// The default value for <see cref="MultiAssertAttribute"/> is -1.
        /// </summary>
        public int Order { get; set; } = -1;
    }
}