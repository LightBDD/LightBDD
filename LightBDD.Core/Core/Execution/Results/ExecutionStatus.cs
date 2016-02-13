namespace LightBDD.Core.Execution.Results
{
    /// <summary>
    /// Represents status of test / scenario.
    /// </summary>
    public enum ExecutionStatus
    {
        /// <summary>
        /// Not run yet
        /// </summary>
        NotRun,
        /// <summary>
        /// Passed
        /// </summary>
        Passed,
        /// <summary>
        /// Bypassed
        /// </summary>
        Bypassed,
        /// <summary>
        /// Ignored / skipped
        /// </summary>
        Ignored,
        /// <summary>
        /// Failed
        /// </summary>
        Failed
    }
}