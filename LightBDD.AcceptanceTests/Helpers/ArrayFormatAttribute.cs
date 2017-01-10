using System.Collections;
using System.Globalization;
using System.Linq;
using LightBDD.Formatting.Parameters;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class ArrayFormatAttribute : ParameterFormatterAttribute
    {
        public override string Format(CultureInfo culture, object parameter)
        {
            return string.Join(", ", ((IEnumerable)parameter).OfType<object>());
        }
    }
}