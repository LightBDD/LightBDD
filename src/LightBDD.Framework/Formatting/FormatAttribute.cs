using System.Globalization;
using LightBDD.Core.Formatting.Parameters;

namespace LightBDD.Framework.Formatting
{
    /// <summary>
    /// Attribute allowing to define how step parameter value should be formatted.
    /// </summary>
    public class FormatAttribute : ParameterFormatterAttribute
    {
        private readonly string _format;
        /// <summary>
        /// Constructor allowing to define how step parameter should be formatted.
        /// The <paramref name="format"/> argument represents string.Format() format parameter, where <c>{0}</c> would be a passed parameter instance.
        /// </summary>
        public FormatAttribute(string format)
        {
            _format = format;
        }
        /// <summary>
        /// Formats given <paramref name="parameter"/> value using <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">Culture used in formatting.</param>
        /// <param name="parameter">Parameter to format.</param>
        /// <returns></returns>
        public override string Format(CultureInfo culture, object parameter)
        {
            return string.Format(culture, _format, parameter);
        }
    }
}