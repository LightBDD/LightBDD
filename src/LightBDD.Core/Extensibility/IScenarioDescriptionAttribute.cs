namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Scenario description attribute interface that can be applied on scenario test methods.
    /// May be used to enrich scenario methods with a description like ""
    /// or similar, that would be used by progress notifier and would be included in summary.
    ///
    /// If given implementation supports alternative description attributes, and both are applied on method, this one would be used.
    /// </summary>
    public interface IScenarioDescriptionAttribute
    {
        /// <summary>
        /// Scenario description.
        /// </summary>
        string Description { get; }
    }
}