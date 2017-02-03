using System.Globalization;
using LightBDD.Core.Formatting.Parameters;

namespace LightBDD.Framework.Formatting.Parameters
{
    /// <summary>
    /// Attribute allowing to define formatting rules for step parameter of boolean type.
    /// </summary>
    public class FormatBooleanAttribute : ParameterFormatterAttribute
    {
        private readonly string _trueValue;
        private readonly string _falseValue;

        /// <summary>
        /// Constructor allowing to associate string constants that would be used for <c>true</c> and <c>false</c> value.
        /// </summary>
        /// <param name="trueValue">String constant used for <c>true</c> value.</param>
        /// <param name="falseValue">String constant used for <c>false</c> value.</param>
        public FormatBooleanAttribute(string trueValue, string falseValue)
        {
            _trueValue = trueValue;
            _falseValue = falseValue;
        }

        /// <summary>
        /// Formats given <paramref name="parameter"/> value using <paramref name="culture"/>.
        /// </summary>
        /// <param name="culture">Culture used in formatting.</param>
        /// <param name="parameter">Parameter to format.</param>
        /// <returns></returns>
        public override string Format(CultureInfo culture, object parameter)
        {
            return (bool)parameter ? _trueValue : _falseValue;
        }
    }
}