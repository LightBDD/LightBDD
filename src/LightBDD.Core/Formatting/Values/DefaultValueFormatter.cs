using System.Globalization;

namespace LightBDD.Core.Formatting.Values
{
    /// <summary>
    /// Default implementation of <see cref="IValueFormatter"/> using <see cref="string.Format(System.IFormatProvider,string,object[])"/> to format provided value.
    /// </summary>
    public class DefaultValueFormatter : IValueFormatter
    {
        private DefaultValueFormatter()
        {
        }

        /// <summary>
        /// Default instance.
        /// </summary>
        public static DefaultValueFormatter Instance { get; } = new DefaultValueFormatter();

        /// <summary>
        /// Formats value provided with <paramref name="value"/> parameter using <see cref="string.Format(System.IFormatProvider,string,object[])"/> and current <see cref="CultureInfo"/> provided by <paramref name="formattingService"/>.
        /// </summary>
        /// <param name="value">Value to format.</param>
        /// <param name="formattingService">Formatting service.</param>
        /// <returns>Formatted value.</returns>
        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Format(formattingService.GetCultureInfo(), "{0}", value);
        }
    }
}