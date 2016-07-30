using System;
using LightBDD.Configuration;
using LightBDD.Integration.XUnit2;
using Xunit.Sdk;

namespace LightBDD
{
    [TestFrameworkDiscoverer("LightBDD.Integration.XUnit2.Customization.TestFrameworkTypeDiscoverer", "LightBDD.Integration.XUnit2")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestFrameworkAttribute
    {
        internal void SetUp()
        {
            var configuration = new LightBddConfiguration();
            OnConfiguration(configuration);
            XUnit2FeatureCoordinator.InstallSelf(configuration);
            BeforeLightBddTests();
        }

        internal void TearDown()
        {
            XUnit2FeatureCoordinator.GetInstance().Dispose();
            AfterLightBddTests();
        }

        protected virtual void OnConfiguration(LightBddConfiguration configuration)
        {
        }

        protected virtual void BeforeLightBddTests()
        {
        }

        protected virtual void AfterLightBddTests()
        {
        }
    }
}