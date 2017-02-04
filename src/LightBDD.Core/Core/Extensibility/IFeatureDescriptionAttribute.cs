namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Feature description attribute interface that can be applied on feature test class.
    /// May be used to enrich feature class with description like "In order to... As a... I want to..."
    /// or similar, that would be used by progress notifier and would be included in summary.
    ///
    /// If given implementation supports alternative description attributes, and both are applied on class, this one would be used.
    /// </summary>
    public interface IFeatureDescriptionAttribute
    {
        /// <summary>
        /// Feature description.
        /// </summary>
        string Description { get; }
    }
}