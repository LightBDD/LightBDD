using LightBDD.Core.Formatting.NameDecorators;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing step name metadata.
    /// </summary>
    public interface IStepNameInfo : INameInfo
    {
        /// <summary>
        /// Formats step name using provided decorator.
        /// </summary>
        /// <param name="decorator">Decorator.</param>
        /// <returns>Formatted name.</returns>
        string Format(IStepNameDecorator decorator);
        /// <summary>
        /// Returns step type name.
        /// </summary>
        string StepTypeName { get; }
    }
}