using LightBDD.Core.Configuration;

namespace LightBDD.Core.UnitTests.Helpers;

internal class TestLightBddConfiguration
{
    public static void SetTestDefaults(LightBddConfiguration cfg)
    {
        //TODO: review to avoid any overrides
        cfg.ExceptionHandlingConfiguration().UpdateExceptionDetailsFormatter(e => $"{e.GetType().Namespace}.{e.GetType().Name}: {e.Message}");
    }
}