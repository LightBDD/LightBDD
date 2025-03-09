#if !NETCOREAPP
using System.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;

namespace LightBDD.TUnit.UnitTests
{
    [FeatureDescription(@"As a developer,
I want runner to support tests requiring cross domain communication,
So that I can write tests accessing application settings")]
    public class Cross_domain_tests : FeatureFixture
    {
        [Scenario]
        public async Task Runner_should_allow_retrieving_application_settings()
        {
            await Runner.RunScenarioAsync( _ => Assert_setting_has_value("foo"));
        }

        private async Task Assert_setting_has_value(string value)
        {
            await Assert.That(value).IsEqualTo(Settings.Default.Setting);
        }

        private class Settings : ApplicationSettingsBase
        {
            public static Settings Default { get; } = (Settings)Synchronized(new Settings());

            [ApplicationScopedSetting]
            [DebuggerNonUserCode]
            [DefaultSettingValue("foo")]
            public string Setting => (string)this["Setting"];
        }
    }
}
#endif