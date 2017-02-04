namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Scenario category attribute interface that can be applied on scenario test method.
    /// May be used to associate scenarios with specific categories.
    /// It is possible to apply multiple categories on given scenario.
    ///
    /// If given implementation supports alternative category attributes, and both are applied on scenario method, all of them would be used.
    /// </summary>
    public interface IScenarioCategoryAttribute
    {
        /// <summary>
        /// Scenario category name.
        /// </summary>
        string Category { get; }
    }
}