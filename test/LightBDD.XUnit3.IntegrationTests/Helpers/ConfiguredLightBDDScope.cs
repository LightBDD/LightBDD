using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.XUnit3.IntegrationTests.Helpers;
using Xunit.v3;

[assembly: TestPipelineStartup(typeof(ConfiguredLightBDDScope))]
namespace LightBDD.XUnit3.IntegrationTests.Helpers
{
    public class ConfiguredLightBDDScope : LightBddScope
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration.ProgressNotifierConfiguration()
                .Clear()
                .Append(ScenarioProgressCapture.Instance);

            configuration.ReportWritersConfiguration()
                .Clear();
        }
    }
}
