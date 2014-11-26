namespace LightBDD.AcceptanceTests.Helpers
{
    internal class SelectedFormatAttribute : ParameterFormatterAttribute
    {
        public override string Format(object parameter)
        {
            return ((bool)parameter) ? "selected" : "unselected";
        }
    }
}