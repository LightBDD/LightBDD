namespace LightBDD.Core.Formatting.Values.Implementation
{
    internal class AsStringFormatter : IValueFormatter
    {
        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            return (string) value;
        }
    }
}