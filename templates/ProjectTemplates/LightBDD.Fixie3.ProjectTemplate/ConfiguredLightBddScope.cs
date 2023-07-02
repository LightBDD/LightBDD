using LightBDD.Core.Configuration;
using LightBDD.Fixie3;

namespace $safeprojectname$
{
    internal class ConfiguredLightBddScope : LightBddScope
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            // LightBDD configuration
        }

        protected override void OnSetUp()
        {
            // code executed before any scenarios
        }

        protected override void OnTearDown()
        {
            // code executed after all scenarios
        }
    }
}
