using System;
using LightBDD.Configuration;
using LightBDD.Integration.NUnit3;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace LightBDD
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestAction
    {
        public void BeforeTest(ITest test)
        {
            var configuration = new LightBddConfiguration();
            OnConfiguration(configuration);
            NUnit3FeatureCoordinator.InstallSelf(configuration);
            BeforeLightBddTests();
        }

        protected virtual void OnConfiguration(LightBddConfiguration configuration)
        {
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