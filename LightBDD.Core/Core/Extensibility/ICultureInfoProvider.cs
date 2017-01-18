using System.Globalization;

namespace LightBDD.Core.Extensibility
{
    //TODO: add tests
    public interface ICultureInfoProvider
    {
        CultureInfo GetCultureInfo();
    }
}