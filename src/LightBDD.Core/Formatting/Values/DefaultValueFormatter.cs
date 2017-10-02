namespace LightBDD.Core.Formatting.Values
{
    public class DefaultValueFormatter : IValueFormatter
    {
        private DefaultValueFormatter()
        {
        }

        public static DefaultValueFormatter Instance { get; } = new DefaultValueFormatter();

        public string FormatValue(object value, IValueFormattingService formattingService)
        {
            return string.Format(formattingService.GetCultureInfo(), "{0}", value);
        }
    }
}