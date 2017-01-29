using System;
using LightBDD.Core.Configuration;
using LightBDD.Integration.NUnit3;
using LightBDD.Notification.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace LightBDD
{
    /// <summary>
    /// An attribute allowing to initialize and finalize LightBDD in NUnit 3 framework.
    /// 
    /// The <c>[assembly:LightBddScope]</c> attribute has to be present in assembly containing LightBDD tests.
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestAction
    {
        /// <summary>Executed before each test is run</summary>
        /// <param name="test">The test that is going to be run.</param>
        public void BeforeTest(ITest test)
        {
            NUnit3FeatureCoordinator.InstallSelf(Configure());
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
                .UpdateNotifier(NUnit3ProgressNotifier.CreateFeatureProgressNotifier());

            configuration.Get<ScenarioProgressNotifierConfiguration>()
                .UpdateNotifierProvider(NUnit3ProgressNotifier.CreateScenarioProgressNotifier);

            OnConfigure(configuration);
            return configuration;
        }

        /// <summary>Executed after each test is run</summary>
        /// <param name="test">The test that has just been run.</param>
        public void AfterTest(ITest test)
        {
            NUnit3FeatureCoordinator.GetInstance().Dispose();
        }

        /// <summary>Provides the target for the action attribute</summary>
        /// <returns>The target for the action attribute</returns>
        public ActionTargets Targets => ActionTargets.Suite;
    }
}