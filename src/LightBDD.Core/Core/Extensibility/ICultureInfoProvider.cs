using System.Globalization;

namespace LightBDD.Core.Extensibility
{
    public interface ICultureInfoProvider
    {
        CultureInfo GetCultureInfo();
    }
}