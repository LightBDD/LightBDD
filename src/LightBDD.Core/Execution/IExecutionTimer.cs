namespace LightBDD.Core.Execution
{
    /// <summary>
    /// Execution timer interface.
    /// </summary>
    public interface IExecutionTimer
    {
        /// <summary>
        /// Returns current execution time.
        /// </summary>
        EventTime GetTime();
    }
}
