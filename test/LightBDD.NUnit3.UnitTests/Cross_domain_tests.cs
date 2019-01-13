#if !NETCOREAPP1_1
using System.Configuration;
using System.Diagnostics;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using NUnit.Framework;

namespace LightBDD.NUnit3.UnitTests
{
    [FeatureDescription(@"As a developer,
I want runner to support tests requiring cross domain communication,
So that I can write tests accessing application settings")]
    public class Cross_domain_tests : FeatureFixture
    {
        [Scenario]
        public void Runner_should_allow_retrieving_application_settings()
        {
            Runner.RunScenario(
                _ => Assert_setting_has_value("foo"));
        }

        private void Assert_setting_has_value(string value)
        {
            Assert.AreEqual(value, Settings.Default.Setting);
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