using System.Collections.Generic;
using LightBDD.Core.Formatting.NameDecorators;

namespace LightBDD.Core.Metadata
{
    /// <summary>
    /// Interface describing name metadata.
    /// </summary>
    public interface INameInfo
    {
        /// <summary>
        /// Formats name using <see cref="StepNameDecorators.Default"/> step name decorator.
        /// </summary>
        /// <returns>Formatted name.</returns>
        string ToString();
        /// <summary>
        /// Formats name using provided decorator.
        /// </summary>
        /// <param name="decorator">Decorator.</param>
        /// <returns>Formatted name.</returns>
        string Format(INameDecorator decorator);
        /// <summary>
        /// Returns name format.
        /// </summary>
        string NameFormat { get; }
        /// <summary>
        /// Returns name parameters used in formatting or empty collection if name is not parameterized.
        /// </summary>
        IEnumerable<INameParameterInfo> Parameters { get; }
    }
}