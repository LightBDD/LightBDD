using System.Globalization;
using LightBDD.Formatting.Parameters;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class SelectedFormatAttribute : ParameterFormatterAttribute
    {
        public override string Format(CultureInfo culture, object parameter)
        {
            return ((bool)parameter) ? "selected" : "unselected";
        }
    }
}