using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.XUnit2.IntegrationTests.Helpers;
using Xunit.Sdk;

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

        protected override void OnSetUp()
        {
            DiagnosticMessageSink.OnMessage(new DiagnosticMessage("OnSetUp"));
        }

        protected override void OnTearDown()
        {
            DiagnosticMessageSink.OnMessage(new DiagnosticMessage("OnTearDown"));
        }
    }
}
