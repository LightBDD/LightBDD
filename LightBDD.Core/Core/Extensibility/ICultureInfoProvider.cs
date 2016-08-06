using System.Globalization;

namespace LightBDD.Core.Extensibility
{
    //TODO: add Extension that would honor test culture info in async scenarios
    //TODO: add tests
    public interface ICultureInfoProvider
    {
        CultureInfo GetCultureInfo();
    }
}