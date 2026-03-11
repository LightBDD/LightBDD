using System;
using System.Threading.Tasks;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.XUnit3.Configuration;
using LightBDD.XUnit3.Implementation;
using Xunit.Sdk;
using Xunit.v3;

namespace LightBDD.XUnit3
{
    /// <summary>
    /// A class allowing to initialize and finalize LightBDD in xUnit v3 framework.
    /// 
    /// It has to be registered using <c>[assembly:TestPipelineStartup(typeof(MyScope))]</c> attribute, where <c>MyScope</c> derives from <see cref="LightBddScopeAttribute"/>.
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method,
    /// as well as execute code before any test and after all tests by overriding the <see cref="OnSetUp"/>() / <see cref="OnTearDown"/>() methods.
    /// </summary>
    public class LightBddScopeAttribute : Attribute, ITestPipelineStartup
    {
        /// <summary>
        /// Called by xUnit v3 at the start of the test pipeline.
        /// Initializes LightBDD configuration and calls <see cref="OnSetUp"/>.
        /// </summary>
        ValueTask ITestPipelineStartup.StartAsync(IMessageSink diagnosticMessageSink)
        {
            XUnit3FeatureCoordinator.InstallSelf(Configure());
            OnSetUp();
            return default;
        }

        /// <summary>
        /// Called by xUnit v3 at the end of the test pipeline.
        /// Calls <see cref="OnTearDown"/> and disposes the LightBDD coordinator.
        /// </summary>
        ValueTask ITestPipelineStartup.StopAsync()
        {
            try
            {
                OnTearDown();
            }
            finally
            {
                XUnit3FeatureCoordinator.GetInstance().Dispose();
            }
            return default;
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

            configuration.ProgressNotifierConfiguration()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format)
                .UpdateExceptionMessageExtractor(StripDynamicSkipToken);

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

        private static string StripDynamicSkipToken(Exception exception)
        {
            var message = exception.Message;
            var prefix = DynamicSkipToken.Value;
            return message.StartsWith(prefix, StringComparison.Ordinal)
                ? message.Substring(prefix.Length)
                : message;
        }
    }
}
