using System.Globalization;

namespace LightBDD.Core.Formatting.Parameters
{
    public interface IParameterFormatter
    {
        string Format(CultureInfo culture, object value);
    }
}