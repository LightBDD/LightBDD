using System.Globalization;

namespace LightBDD.Core.Formatting.Values
{
    /// <summary>
    /// Interface allowing to define a formatting method for objects.
    /// </summary>
    public interface IValueFormatter
    {
        /// <summary>
        /// Formats provided <paramref name="value"/> and returns it's string representation.
        /// The provided <paramref name="formattingService"/> can be used to obtain current <see cref="CultureInfo"/> if needed. It can be also used to format inner values if formatted type is collection or complex object.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="formattingService">Formatting service allowing to retrieve current <see cref="CultureInfo"/> or format inner values of provided object.</param>
        /// <returns>Formatted value.</returns>
        string FormatValue(object value, IValueFormattingService formattingService);
    }
}