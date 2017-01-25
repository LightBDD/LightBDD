using System;
using LightBDD.Core.Configuration;
using LightBDD.Integration.XUnit2;
using LightBDD.Notification.Configuration;
using Xunit.Sdk;

namespace LightBDD
{
    [TestFrameworkDiscoverer("LightBDD.Integration.XUnit2.Customization.TestFrameworkTypeDiscoverer", "LightBDD.Integration.XUnit2")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestFrameworkAttribute
    {
        internal void SetUp()
        {
            XUnit2FeatureCoordinator.InstallSelf(Configure());
            BeforeLightBddTests();
        }

        internal void TearDown()
        {
            XUnit2FeatureCoordinator.GetInstance().Dispose();
            AfterLightBddTests();
        }

        protected virtual void OnConfigure(LightBddConfiguration configuration)
        {
        }

        protected virtual void BeforeLightBddTests()
        {
        }

        protected virtual void AfterLightBddTests()
        {
        }

        private LightBddConfiguration Configure()
        {
            var configuration = new LightBddConfiguration();

            configuration.Get<FeatureProgressNotifierConfiguration>()
                .UpdateNotifier(XUnit2ProgressNotifier.CreateFeatureProgressNotifier());

            configuration.Get<ScenarioProgressNotifierConfiguration>()
                .UpdateNotifierProvider<ITestOutputProvider>(fixture => XUnit2ProgressNotifier.CreateScenarioProgressNotifier(fixture.TestOutput));

            OnConfigure(configuration);
            return configuration;
        }
    }
}