using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;

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

        /// <inheritdoc />
        public override string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Format(formattingService.GetCultureInfo(), _format, value);
        }
    }
}