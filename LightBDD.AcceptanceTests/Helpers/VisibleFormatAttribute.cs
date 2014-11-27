using LightBDD.Formatting.Parameters;

namespace LightBDD.AcceptanceTests.Helpers
{
    internal class VisibleFormatAttribute : ParameterFormatterAttribute
    {
        public override string Format(object parameter)
        {
            return ((bool)parameter) ? "visible" : "invisible";
        }
    }
}