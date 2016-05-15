using System;
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
            NUnit3FeatureCoordinator.InstallSelf();
            BeforeLightBddTests();
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