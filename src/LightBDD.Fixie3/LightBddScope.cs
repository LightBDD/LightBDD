using System;
using System.Linq;
using Fixie;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Fixie3.Configuration;
using LightBDD.Fixie3.Implementation;
using LightBDD.Framework.Configuration;

namespace LightBDD.Fixie3
{
    /// <summary>
    /// LightBddScope class allowing to initialize and finalize LightBDD in Fixie framework.
    ///
    /// Example showing how to initialize LightBDD in Fixie framework:
    /// <example>
    /// class ConfiguredLightBddScope: LightBddScope { }
    /// </example>
    ///
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method,
    /// as well as execute code before any test and after all tests by overriding the <see cref="OnSetUp"/>() / <see cref="OnTearDown"/>() methods.
    /// </summary>
    public class LightBddScope : ITestProject
    {
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

        internal void SetUp()
        {
            FixieFeatureCoordinator.InstallSelf(Configure());
            OnSetUp();
        }

        internal void TearDown()
        {
            try
            {
                OnTearDown();
            }
            finally
            {
                FixieFeatureCoordinator.GetInstance().Dispose();
            }
        }

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

        void ITestProject.Configure(TestConfiguration configuration, TestEnvironment environment)
        {
            var categories = ArgsParser.ParseCategories(environment).ToArray();
            configuration.Conventions.Add(new LightBddDiscoveryConvention(categories), new LightBddExecutionConvention(this));
        }
    }
}