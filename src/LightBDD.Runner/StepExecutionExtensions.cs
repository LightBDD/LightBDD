using LightBDD.Core.Results;
using LightBDD.Framework;
using LightBDD.Runner.Implementation;

namespace LightBDD.Runner
{
    /// <summary>
    /// Extension class for <see cref="StepExecution"/> allowing to ignore currently running steps.
    /// </summary>
    public static class StepExecutionExtensions
    {
        /// <summary>
        /// Aborts execution of current scenario, marking scenario ignored.
        /// The currently executed step as well as scenario status becomes <see cref="ExecutionStatus.Ignored"/> in LightBDD reports, while test itself would be marked as 'Skipped' in xUnit.
        /// </summary>
        /// <param name="execution">Current step execution instance.</param>
        /// <param name="reason">Ignore reason.</param>
        public static void IgnoreScenario(this StepExecution execution, string reason)
        {
            throw new IgnoreException(reason);
        }
    }
}