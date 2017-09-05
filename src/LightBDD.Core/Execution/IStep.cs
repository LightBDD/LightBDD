using System;
using LightBDD.Core.Metadata;

namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Interface describing current step, providing step information details and ability to comment it.
    /// </summary>
    public interface IStep
    {
        /// <summary>
        /// Step information details.
        /// </summary>
        IStepInfo Info { get; }
        /// <summary>
        /// Annotates step with comment.
        /// It is possible to comment step many times.
        /// </summary>
        /// <param name="comment">Comment.</param>
        void Comment(string comment);
        /// <summary>
        /// Allows to install function specifying if given exception should abort execution of current step group and propagate exception to parent or continue execution.
        /// If <paramref name="shouldAbortExecutionFn"/> function return false, the step result would be set accordingly to the exception severity but other steps within group would be executed.
        /// Any suppressed exceptions would be provided to parent step/scenario after step group execution.
        /// </summary>
        /// <param name="shouldAbortExecutionFn">Function returning true if given exception should abort further execution or should allow to execute further steps within group.</param>
        void ConfigureExecutionAbortOnException(Func<Exception, bool> shouldAbortExecutionFn);
    }
}