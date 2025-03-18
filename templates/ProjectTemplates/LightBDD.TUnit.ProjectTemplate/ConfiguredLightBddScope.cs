using $safeprojectname$;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.TUnit;

[assembly: ConfiguredLightBddScope]

namespace $safeprojectname$
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            // LightBDD configuration
        }

        protected override async ValueTask OnSetUp()
        {
            // code executed before any scenarios
        }

        protected override async ValueTask OnTearDown()
        {
            // code executed after all scenarios
        }
    }
}
