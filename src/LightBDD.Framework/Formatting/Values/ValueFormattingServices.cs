using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.Parameters;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Execution.Coordination;

namespace LightBDD.Framework.Formatting.Values
{
    /// <summary>
    /// Class offering access to current <see cref="IValueFormattingService"/> instance.
    /// </summary>
    public static class ValueFormattingServices
    {
        private static readonly IValueFormattingService Default = new ValueFormattingService(new LightBddConfiguration().WithFrameworkDefaults());

        /// <summary>
        /// Returns current <see cref="IValueFormattingService"/> instance that has been configured or default instance if LightBDD is not initialized yet.
        /// Please note that this property would not include formatters declared with <see cref="ParameterFormatterAttribute"/> attributes on the method parameters.
        /// </summary>
        public static IValueFormattingService Current => FrameworkFeatureCoordinator.TryGetInstance()?.ValueFormattingService ?? Default;
    }
}