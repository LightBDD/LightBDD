namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing scenario step metadata.
    /// </summary>
    public interface IStepInfo
    {
        /// <summary>
        /// Returns step name.
        /// </summary>
        IStepNameInfo Name { get; }
        /// <summary>
        /// Returns step number in all steps belonging to scenario.
        /// </summary>
        int Number { get; }
        /// <summary>
        /// Returns total number of steps in scenario.
        /// </summary>
        int Total { get; }
    }
}