﻿#if !NETCOREAPP1_1
using System.Configuration;
using System.Diagnostics;
using LightBDD.Framework.Scenarios.Extended;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.XUnit2.UnitTests
{
    public class Cross_domain_tests : FeatureFixture
    {
        [Scenario]
        public void Runner_should_allow_cross_domain_communication()
        {
            Runner.RunScenario(
                _ => Assert_setting_has_value("foo"));
        }
        private void Assert_setting_has_value(string value)
        {
            Assert.Equal(value, Settings.Default.Setting);
        }

        private class Settings : ApplicationSettingsBase
        {
            public static Settings Default { get; } = (Settings)Synchronized(new Settings());

            [ApplicationScopedSetting]
            [DebuggerNonUserCode]
            [DefaultSettingValue("foo")]
            public string Setting => (string)this["Setting"];
        }

        public Cross_domain_tests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
#endif