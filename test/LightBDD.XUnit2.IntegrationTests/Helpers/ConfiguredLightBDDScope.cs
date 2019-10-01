using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.XUnit2.IntegrationTests.Helpers;

[assembly: ConfiguredLightBDDScope]
namespace LightBDD.XUnit2.IntegrationTests.Helpers
{
    public class ConfiguredLightBDDScope : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.FeatureProgressNotifierConfiguration()
                .ClearNotifiers();
            configuration.ScenarioProgressNotifierConfiguration()
                .ClearNotifierProviders()
                .UpdateNotifierProvider(() => ScenarioProgressCapture.Instance);
            configuration.ReportWritersConfiguration()
                .Clear();
        }
    }
}
