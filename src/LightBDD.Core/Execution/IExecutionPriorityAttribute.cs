namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Execution priority attribute interface that can be applied on scenario methods to change their execution priority.
    /// </summary>
    public interface IExecutionPriorityAttribute
    {
        /// <summary>
        /// Returns execution priority of the annotated scenario, where higher values means higher priority to execute the scenario.
        /// </summary>
        int Priority { get; }
    }
}
