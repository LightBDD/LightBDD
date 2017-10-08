using System.Globalization;

namespace LightBDD.Core.Formatting.Values
{
    /// <summary>
    /// Interface allowing to define a formatting method for object implementing this interface.
    /// </summary>
    public interface ISelfFormattable
    {
        /// <summary>
        /// Returns string representation of current instance value.
        /// The provided <paramref name="formattingService"/> can be used to obtain current <see cref="CultureInfo"/> if needed. It can be also used to format inner values if formatted type is collection or complex object.
        /// </summary>
        /// <param name="formattingService">Formatting service allowing to retrieve current <see cref="CultureInfo"/> or format inner values of provided object.</param>
        /// <returns>Formatted value.</returns>
        string Format(IValueFormattingService formattingService);
    }
}