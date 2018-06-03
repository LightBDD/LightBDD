using System.Diagnostics;
using System.Globalization;

namespace LightBDD.Core.Extensibility
{
    /// <summary>
    /// Default implementation of <see cref="ICultureInfoProvider"/> returning <c>CultureInfo.InvariantCulture</c>.
    /// </summary>
    [DebuggerStepThrough]
    public class DefaultCultureInfoProvider : ICultureInfoProvider
    {
        /// <summary>
        /// Returns CultureInfo.InvariantCulture that should be used for text formatting.
        /// </summary>
        /// <returns>CultureInfo.InvariantCulture.</returns>
        public CultureInfo GetCultureInfo()
        {
            return CultureInfo.InvariantCulture;
        }
    }
}