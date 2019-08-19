namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing scenario step metadata.
    /// </summary>
    public interface IStepInfo : IMetadataInfo
    {
        /// <summary>
        /// Returns the step name.
        /// </summary>
        new IStepNameInfo Name { get; }
        /// <summary>
        /// The step parent that could be either <see cref="IScenarioInfo"/> or <see cref="IStepInfo"/>.
        /// </summary>
        IMetadataInfo Parent { get; }
        /// <summary>
        /// Returns group prefix if step belongs to a composite step, or <see cref="string.Empty"/> if step belongs to scenario.
        /// The group prefix is in form of <c>Z.Y.X.</c>, where X,Y,Z corresponds to parent, grand parent, grand-grand parent, etc. step number.
        /// </summary>
        string GroupPrefix { get; }
        /// <summary>
        /// Returns step number in all steps belonging to scenario or composite step.
        /// </summary>
        int Number { get; }
        /// <summary>
        /// Returns total number of steps in scenario or composite step.
        /// </summary>
        int Total { get; }
    }
}