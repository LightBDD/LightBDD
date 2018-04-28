using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.Values;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Execution.Coordination;

namespace LightBDD.Framework.Formatting.Values
{
    public static class ValueFormattingServices
    {
        private static readonly IValueFormattingService Default = new ValueFormattingService(new LightBddConfiguration().WithFrameworkDefaults());

        public static IValueFormattingService Current => FrameworkFeatureCoordinator.TryGetInstance()?.ValueFormattingService ?? Default;
    }
}