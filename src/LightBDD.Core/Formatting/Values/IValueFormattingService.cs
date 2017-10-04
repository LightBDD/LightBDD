using System.Globalization;

namespace LightBDD.Core.Formatting.Values
{
    /// <summary>
    /// Interface describing a formatting service being capable to format value of any type, including nulls as well as providing current <see cref="CultureInfo"/>.
    /// </summary>
    public interface IValueFormattingService
    {
        /// <summary>
        /// Formats value provided by <paramref name="value"/> parameter.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <returns>Formatted string representation of the provided value.</returns>
        string FormatValue(object value);

        /// <summary>
        /// Returns current <see cref="CultureInfo"/> that will be used to format values.
        /// </summary>
        /// <returns>Current <see cref="CultureInfo"/> instance.</returns>
        CultureInfo GetCultureInfo();
    }
}