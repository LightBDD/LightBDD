using System;

namespace LightBDD.Core.Formatting.Values
{
    /// <summary>
    /// Interfce allowing to define a formatting method for objects of types that are accepted by <see cref="CanFormat"/> method.
    /// </summary>
    public interface IConditionalValueFormatter : IValueFormatter
    {
        /// <summary>
        /// Returns true if type specified by <paramref name="type"/> parameter is supported by formatter.
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>True if <paramref name="type"/> is supported by formatter.</returns>
        bool CanFormat(Type type);
    }
}