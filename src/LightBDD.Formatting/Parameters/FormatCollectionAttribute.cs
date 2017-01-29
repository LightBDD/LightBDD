using System.Collections;
using System.Globalization;
using System.Linq;
using LightBDD.Core.Formatting.Parameters;

namespace LightBDD.Formatting.Parameters
{
    /// <summary>
    /// Attribute allowing to define formatting rules for method parameter of type implementing <see cref="IEnumerable"/> interface.
    /// </summary>
    public class FormatCollectionAttribute : ParameterFormatterAttribute
    {
        private readonly string _separator;
        private readonly string _valueFormat;

        /// <summary>
        /// Constructor allowing to specify separator and value format.
        /// </summary>
        /// <param name="separator">Value separator. If <c>null</c>, <c>", "</c> will be used.</param>
        /// <param name="valueFormat">Value format. If <c>null</c>, <c>"{0}"</c> will be used.</param>
        public FormatCollectionAttribute(string separator = null, string valueFormat = null)
        {
            _separator = separator ?? ", ";
            _valueFormat = valueFormat ?? "{0}";
        }

        /// <summary>
        /// Formats given <paramref name="parameter"/> value using <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">Culture used in formatting.</param>
        /// <param name="parameter">Parameter to format.</param>
        /// <returns></returns>
        public override string Format(CultureInfo culture, object parameter)
        {
            return string.Join(_separator, ((IEnumerable)parameter).Cast<object>().Select(o => string.Format(culture, _valueFormat, o)));
        }
    }
}