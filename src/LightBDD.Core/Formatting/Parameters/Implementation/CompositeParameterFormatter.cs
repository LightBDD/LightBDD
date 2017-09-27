using System.Globalization;
using System.Linq;

namespace LightBDD.Core.Formatting.Parameters.Implementation
{
    class CompositeParameterFormatter : IParameterFormatter
    {
        private readonly IParameterFormatter _defaultFormatter;
        private readonly IConditionalParameterFormatter[] _conditionalFormatters;

        public CompositeParameterFormatter(IParameterFormatter defaultFormatter, params IConditionalParameterFormatter[] conditionalFormatters)
        {
            _defaultFormatter = defaultFormatter;
            _conditionalFormatters = conditionalFormatters;
        }

        public string Format(CultureInfo culture, object value)
        {
            var formatter = _conditionalFormatters.FirstOrDefault(x => x.CanFormat(value)) ?? _defaultFormatter;
            return formatter.Format(culture, value);
        }
    }
}