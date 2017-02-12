using LightBDD.Core.Configuration;
using LightBDD.NUnit3;

[assembly: $rootnamespace$.ConfiguredLightBddScope]
namespace $rootnamespace$
{
    class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            // custom configuration
        }
    }
}