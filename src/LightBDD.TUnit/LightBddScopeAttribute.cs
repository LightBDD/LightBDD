using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.TUnit.Configuration;
using LightBDD.TUnit.Implementation;
using TUnit.Core.Interfaces;

namespace LightBDD.TUnit
{
    /// <summary>
    /// An attribute allowing to initialize and finalize LightBDD in NUnit framework.
    /// 
    /// The <c>[assembly:LightBddScope]</c> attribute has to be present in assembly containing LightBDD tests.
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method,
    /// as well as execute code before any test and after all tests by overriding the <see cref="OnSetUp"/>() / <see cref="OnTearDown"/>() methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, IFirstTestInTestSessionEventReceiver, ILastTestInTestSessionEventReceiver
    {
        /// <summary>Executed before all tests</summary>
        /// <param name="sessionContext"></param>
        /// <param name="testContext">The test that is going to be run.</param>
        public ValueTask OnFirstTestInTestSession(TestSessionContext sessionContext, TestContext testContext)
        {
            TUnitFeatureCoordinator.InstallSelf(Configure());
            return OnSetUp();
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
        protected virtual ValueTask OnSetUp() => default;

        /// <summary>
        /// Allows to execute additional cleanup just after LightBDD scope termination
        /// </summary>
        protected virtual ValueTask OnTearDown() => default;

        private LightBddConfiguration Configure()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            configuration.ProgressNotifierConfiguration()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format);

            try
            {
                OnConfigure(configuration);
            }
            catch (Exception ex)
            {
                configuration.ExecutionExtensionsConfiguration().CaptureFrameworkInitializationException(ex);
            }
            return configuration;
        }

        /// <summary>Executed after each test is run</summary>
        /// <param name="sessionContext"></param>
        /// <param name="testContext">The test that has just been run.</param>
        public ValueTask OnLastTestInTestSession(TestSessionContext sessionContext, TestContext testContext)
        {
            try
            {
                return OnTearDown();
            }
            finally
            {
                TUnitFeatureCoordinator.GetInstance().Dispose();
            }
        }

        /// <summary>
        /// The order of the test event receiver.
        /// </summary>
        public int Order { get; }
    }
}