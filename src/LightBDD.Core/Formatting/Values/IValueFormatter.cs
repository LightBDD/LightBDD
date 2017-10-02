namespace LightBDD.Core.Formatting.Values
{
    public interface IValueFormatter
    {
        string FormatValue(object value, IValueFormattingService formattingService);
    }
}