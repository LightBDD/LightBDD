using System;
using LightBDD.Core.Configuration;
using LightBDD.Integration.NUnit3;
using LightBDD.Notification.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace LightBDD
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            NUnit3FeatureCoordinator.InstallSelf(Configure());
            BeforeLightBddTests();
        }

        protected virtual void OnConfigure(LightBddConfiguration configuration)
        {
        }

        private LightBddConfiguration Configure()
        {
            var configuration = new LightBddConfiguration();

            configuration.Get<FeatureProgressNotifierConfiguration>()
                .UpdateNotifier(NUnit3ProgressNotifier.CreateFeatureProgressNotifier());

            configuration.Get<ScenarioProgressNotifierConfiguration>()
                .UpdateNotifierProvider(NUnit3ProgressNotifier.CreateScenarioProgressNotifier);

            OnConfigure(configuration);
            return configuration;
        }

        protected virtual void BeforeLightBddTests()
        {
        }

        public void AfterTest(ITest test)
        {
            NUnit3FeatureCoordinator.GetInstance().Dispose();
            AfterLightBddTests();
        }
        protected virtual void AfterLightBddTests()
        {
        }

        public ActionTargets Targets => ActionTargets.Suite;
    }
}