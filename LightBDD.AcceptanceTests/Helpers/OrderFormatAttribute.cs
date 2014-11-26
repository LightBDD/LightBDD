namespace LightBDD.AcceptanceTests.Helpers
{
    internal class OrderFormatAttribute : ParameterFormatterAttribute
    {
        public override string Format(object parameter)
        {
            return ((bool)parameter) ? "ascending" : "descending";
        }
    }
}