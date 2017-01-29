using System;
using LightBDD.Core.Configuration;
using LightBDD.Integration.XUnit2;
using LightBDD.Notification.Configuration;
using Xunit.Sdk;

namespace LightBDD
{
    /// <summary>
    /// An attribute allowing to initialize and finalize LightBDD in XUnit 2 framework.
    /// 
    /// The <c>[assembly:LightBddScope]</c> attribute has to be present in assembly containing LightBDD tests.
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method.
    /// </summary>
    [TestFrameworkDiscoverer("LightBDD.Integration.XUnit2.Customization.TestFrameworkTypeDiscoverer", "LightBDD.Integration.XUnit2")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestFrameworkAttribute
    {
        internal void SetUp()
        {
            XUnit2FeatureCoordinator.InstallSelf(Configure());
        }

        internal void TearDown()
        {
            XUnit2FeatureCoordinator.GetInstance().Dispose();
        }

        /// <summary>
        /// Allows to customize LightBDD configuration.
        /// </summary>
        /// <param name="configuration">Configuration to customize.</param>
        protected virtual void OnConfigure(LightBddConfiguration configuration)
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