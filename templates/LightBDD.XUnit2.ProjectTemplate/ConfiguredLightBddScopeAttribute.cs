using LightBDD.Core.Configuration;
using LightBDD.XUnit2;

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