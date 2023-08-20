using LightBDD.Core.Configuration;
using LightBDD.Core.ExecutionContext;
using LightBDD.Core.Extensibility;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Configuration;

namespace LightBDD.Framework.Formatting.Values
{
    /// <summary>
    /// Class offering access to current <see cref="IValueFormattingService"/> instance.
    /// </summary>
    public static class ValueFormattingServices
    {
        //TODO: rework - new configuration may be creating new container and this should be avoided
        private static readonly IValueFormattingService Default = new ValueFormattingService(new ValueFormattingConfiguration().RegisterFrameworkDefaultGeneralFormatters(), new DefaultCultureInfoProvider());

        /// <summary>
        /// Returns current <see cref="IValueFormattingService"/> instance that has been configured or default instance if LightBDD is not initialized yet.
        /// Please note that this property would not include formatters declared with <see cref="ParameterFormatterAttribute"/> attributes on the method parameters.
        /// </summary>
        //TODO: rework
        public static IValueFormattingService Current => LightBddExecutionContext.GetCurrentIfPresent()?.ValueFormattingService ?? Default;
    }
}