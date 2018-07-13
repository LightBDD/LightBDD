using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification.Configuration;
using LightBDD.XUnit2.Configuration;
using LightBDD.XUnit2.Implementation;
using Xunit.Sdk;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// An attribute allowing to initialize and finalize LightBDD in XUnit framework.
    /// 
    /// The <c>[assembly:LightBddScope]</c> attribute has to be present in assembly containing LightBDD tests.
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method,
    /// as well as execute code before any test and after all tests by overriding the <see cref="OnSetUp"/>() / <see cref="OnTearDown"/>() methods.
    /// </summary>
    [TestFrameworkDiscoverer("LightBDD.XUnit2.Implementation.Customization." + nameof(TestFrameworkTypeDiscoverer), "LightBDD.XUnit2")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestFrameworkAttribute
    {
        internal void SetUp()
        {
            XUnit2FeatureCoordinator.InstallSelf(Configure());
            OnSetUp();
        }

        /// <summary>
        /// Allows to execute additional actions after LightBDD scope initialization
        /// </summary>
        protected virtual void OnSetUp() { }

        internal void TearDown()
        {
            try
            {
                OnTearDown();
            }
            finally
            {
                XUnit2FeatureCoordinator.GetInstance().Dispose();
            }
        }

        /// <summary>
        /// Allows to execute additional cleanup just after LightBDD scope termination
        /// </summary>
        protected virtual void OnTearDown()
        {
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
    }
}