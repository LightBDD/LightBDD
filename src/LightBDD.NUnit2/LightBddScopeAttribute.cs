using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification.Configuration;
using LightBDD.NUnit2.Configuration;
using LightBDD.NUnit2.Implementation;
using NUnit.Framework;

namespace LightBDD.NUnit2
{
    /// <summary>
    /// An attribute allowing to initialize and finalize LightBDD in NUnit framework.
    /// 
    /// The <c>[assembly:LightBddScope]</c> attribute has to be present in assembly containing LightBDD tests.
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestAction
    {
        /// <summary>Executed before each test is run</summary>
        /// <param name="testDetails">Provides details about the test that is going to be run.</param>
        public void BeforeTest(TestDetails testDetails)
        {
            NUnit2FeatureCoordinator.InstallSelf(Configure());
            OnSetUp();
        }

        /// <summary>
        /// Allows to customize LightBDD configuration.
        /// </summary>
        /// <param name="configuration">Configuration to customize.</param>
        protected virtual void OnConfigure(LightBddConfiguration configuration)
        {
        }

        /// <summary>
        /// Allows to execute additional actions after LightBDD scope initialization
        /// </summary>
        protected virtual void OnSetUp() { }

        /// <summary>
        /// Allows to execute additional cleanup just after LightBDD scope termination
        /// </summary>
        protected virtual void OnTearDown()
        {
        }

        private LightBddConfiguration Configure()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            configuration.Get<FeatureProgressNotifierConfiguration>()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.Get<ScenarioProgressNotifierConfiguration>()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format);

            OnConfigure(configuration);
            return configuration;
        }

        /// <summary>Executed after each test is run</summary>
        /// <param name="testDetails">Provides details about the test that has just been run.</param>
        public void AfterTest(TestDetails testDetails)
        {
            try
            {
                OnTearDown();
            }
            finally
            {
                NUnit2FeatureCoordinator.GetInstance().Dispose();
            }
        }

        /// <summary>Provides the target for the action attribute</summary>
        /// <returns>The target for the action attribute</returns>
        public ActionTargets Targets => ActionTargets.Suite;
    }
}