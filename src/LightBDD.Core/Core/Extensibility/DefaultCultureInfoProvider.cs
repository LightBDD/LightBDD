using System.Globalization;

namespace LightBDD.Core.Extensibility
{
    public class DefaultCultureInfoProvider : ICultureInfoProvider
    {
        public CultureInfo GetCultureInfo() => CultureInfo.InvariantCulture;
    }
}