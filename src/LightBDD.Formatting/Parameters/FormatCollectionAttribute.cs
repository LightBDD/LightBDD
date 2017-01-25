using System.Collections;
using System.Globalization;
using System.Linq;
using LightBDD.Core.Formatting.Parameters;

namespace LightBDD.Formatting.Parameters
{
    public class FormatCollectionAttribute : ParameterFormatterAttribute
    {
        private readonly string _separator;
        private readonly string _valueFormat;

        public FormatCollectionAttribute(string separator = null, string valueFormat = null)
        {
            _separator = separator ?? ", ";
            _valueFormat = valueFormat ?? "{0}";
        }

        public override string Format(CultureInfo culture, object parameter)
        {
            return string.Join(_separator, ((IEnumerable)parameter).Cast<object>().Select(o => string.Format(culture, _valueFormat, o)));
        }
    }
}