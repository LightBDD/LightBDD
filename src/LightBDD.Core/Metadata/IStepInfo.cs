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
        /// Returns group prefix if step belongs to a step group, or <see cref="string.Empty"/> if step belongs to scenario.
        /// The group prefix is in form of <c>Z.Y.X.</c>, where X,Y,Z corresponds to parent, grand parent, grand-grand parent, etc. step number.
        /// </summary>
        string GroupPrefix { get; }
        /// <summary>
        /// Returns step number in all steps belonging to scenario or step group.
        /// </summary>
        int Number { get; }
        /// <summary>
        /// Returns total number of steps in scenario or step group.
        /// </summary>
        int Total { get; }
    }
}