using System.Globalization;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Interface allowing to configure CultureInfo used in formatting text by LightBDD.
    /// </summary>
    public interface ICultureInfoProvider
    {
        /// <summary>
        /// Returns CultureInfo that should be used for text formatting.
        /// </summary>
        /// <returns>CultureInfo to use.</returns>
        CultureInfo GetCultureInfo();
    }
}