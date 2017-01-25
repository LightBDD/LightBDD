using System.Globalization;
using LightBDD.Core.Formatting.Parameters;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class OrderFormatAttribute : ParameterFormatterAttribute
    {
        public override string Format(CultureInfo culture, object parameter)
        {
            return ((bool)parameter) ? "ascending" : "descending";
        }
    }
}