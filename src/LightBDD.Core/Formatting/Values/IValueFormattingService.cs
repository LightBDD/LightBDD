using System.Globalization;

namespace LightBDD.Core.Formatting.Values
{
    public interface IValueFormattingService
    {
        string FormatValue(object value);
        CultureInfo GetCultureInfo();
    }
}