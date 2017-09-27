using System.Globalization;

namespace LightBDD.Core.Formatting.Parameters.Implementation
{
    class DefaultParameterFormatter : IParameterFormatter
    {
        public string Format(CultureInfo culture, object value)
        {
            return string.Format(culture, "{0}", value ?? FormatSymbols.Instance.NullValue);
        }
    }
}