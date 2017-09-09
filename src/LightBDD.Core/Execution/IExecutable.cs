using System;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Interface allowing to configure executable object (scenario or step).
    /// </summary>
    public interface IExecutable
    {
        /// <summary>
        /// Allows to install function specifying if given exception should abort execution of other sub-steps belonging to current scenario or step.
        /// If <paramref name="shouldAbortExecutionFn"/> function return false, the remaining sub-steps will be executed.
        /// Any suppressed exceptions would be provided to parent step/scenario after step group execution.
        /// If any sub-step execution finish with failed status, an <see cref="AggregateException"/> will be thrown with exceptions of all failed sub-steps.
        /// If none sub-steps failed but there are some with ignored status, the exception of first ignored sub-step would be thrown in order to properly ignore test in underlying test framework.
        /// </summary>
        /// <param name="shouldAbortExecutionFn">Function returning true if given exception should abort further execution or should allow to execute further sub-steps within group.</param>
        void ConfigureExecutionAbortOnSubStepException(Func<Exception, bool> shouldAbortExecutionFn);
    }
}