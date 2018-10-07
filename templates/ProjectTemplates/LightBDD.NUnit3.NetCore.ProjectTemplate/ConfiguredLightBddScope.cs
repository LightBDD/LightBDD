using $safeprojectname$;
using LightBDD.Core.Configuration;
using LightBDD.NUnit3;
using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: ConfiguredLightBddScope]

namespace $safeprojectname$
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
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
