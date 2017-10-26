﻿using System.Configuration;
using System.Diagnostics;
using LightBDD.Framework.Scenarios.Extended;
using NUnit.Framework;

namespace LightBDD.NUnit2.NUnitTests
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
