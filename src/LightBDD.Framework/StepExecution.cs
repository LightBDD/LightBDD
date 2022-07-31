using System;
using System.Threading.Tasks;
using LightBDD.Core.Dependencies;
using LightBDD.Core.Execution;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Reporting;
using LightBDD.Core.Results;

namespace LightBDD.Framework
{
    /// <summary>
    /// Step execution class, allowing to control step execution from method executing step.
    /// </summary>
    public class StepExecution
    {
        /// <summary>
        /// Returns current step execution instance.
        /// Reference by <see cref="Current"/> property enables LightBDD extension packages to add functionality to <see cref="StepExecution"/> with extension methods.
        /// </summary>
        public static StepExecution Current { get; } = new();

        private StepExecution() { }

        /// <summary>
        /// Bypasses currently executed step and continues execution of current scenario, allowing to execute all remaining steps.
        /// The step code located after <c>StepExecution.Current.Bypass()</c> call would not be executed.
        /// <para>
        /// The status of bypassed step would be <see cref="ExecutionStatus.Bypassed"/> and the overall status of scenario would be <see cref="ExecutionStatus.Bypassed"/>,
        /// unless any further step is failed or ignored.
        /// </para>
        /// <para>Scenarios with <see cref="ExecutionStatus.Bypassed"/> status are recognized as successful tests in underlying test framework.</para>
        /// 
        /// <para>The <paramref name="reason"/> argument would be used as step <see cref="IStepResult.StatusDetails"/>, and it would be aggregated in overall scenario <see cref="IScenarioResult.StatusDetails"/> as well.</para>
        /// 
        /// The <see cref="Bypass"/>() method could be used in situations when:
        /// <list type="bullet">
        /// <item><description>It is not possible to implement given step at the moment (no required API is implemented yet), but all other steps are precise enough to prove that scenario is successful, i.e. situation when scenario checks overall and detailed cost of product and one of price component cannot be retrieved.</description></item>
        /// <item><description>Step implementation does not exists, but it is possible to simulate it, so further steps can be executed, i.e. end-to-end tests, where the middle component does not exist yet.</description></item>
        /// <item><description>The required API is not exposed yet, but it is possible to implement a workaround like direct data insert to database.</description></item>
        /// </list>
        /// </summary>
        /// <param name="reason">Bypass reason.</param>
        /// <exception cref="StepBypassException">Bypass exception used to control scenario execution.</exception>
        public void Bypass(string reason) => throw new StepBypassException(reason);

        /// <summary>
        /// Comments currently executed step with a <paramref name="comment"/> text.
        /// The comment would be included in progress notification, as well as in execution reports.
        /// </summary>
        /// <param name="comment">Comment to add. If comment is <c>null</c> or empty, it will not be added.</param>
        public void Comment(string comment)
        {
            if (!string.IsNullOrWhiteSpace(comment))
                ScenarioExecutionContext.CurrentStep.Comment(comment);
        }

        /// <summary>
        /// Retrieves <see cref="IDependencyResolver"/> for currently executed scenario.
        /// Please note that for contextual scenarios or composite steps, it is better to specify <see cref="IDependencyResolver"/> in context constructor.
        /// </summary>
        public IDependencyResolver GetScenarioDependencyResolver() => ScenarioExecutionContext.CurrentScenario.DependencyResolver;

        /// <summary>
        /// Adds the file attachment to the step.
        /// </summary>
        /// <param name="createAttachmentFn">Function creating file attachment by using provided attachments manager.</param>
        /// <returns></returns>
        public Task AttachFile(Func<IFileAttachmentsManager, Task<FileAttachment>> createAttachmentFn)
            => ScenarioExecutionContext.CurrentStep.AttachFile(createAttachmentFn);
    }
}