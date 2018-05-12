using System;
using System.Diagnostics;
using Fixie;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Fixie2.Configuration;
using LightBDD.Fixie2.Implementation;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Notification.Configuration;

namespace LightBDD.Fixie2
{
    /// <summary>
    /// LightBddScope class allowing to initialize and finalize LightBDD in Fixie framework.
    ///
    /// Example showing how to initialize LightBDD in Fixie framework:
    /// <example>
    /// class WithLightBddDiscovery: LightBddDiscoveryConvention { }
    /// class ConfiguredLightBddScope: LightBddScope { }
    /// </example>
    ///
    /// It is possible to customize the LightBDD configuration by overriding the <see cref="OnConfigure"/>() method,
    /// as well as execute code before any test and after all tests by overriding the <see cref="OnSetUp"/>() / <see cref="OnTearDown"/>() methods.
    /// </summary>
    public class LightBddScope : Execution, IDisposable
    {
        [DebuggerStepThrough]
        void Execution.Execute(TestClass testClass)
        {
            testClass.RunCases(c => RunCase(testClass, c));
        }

        [DebuggerStepThrough]
        private void RunCase(TestClass testClass, Case testCase)
        {
            var instance = testClass.Construct();
            try
            {
                TestContextProvider.Initialize(testCase.Method, testCase.Parameters);
                testCase.Execute(instance);
                if (testCase.Exception is IgnoreException)
                    testCase.Skip(testCase.Exception.Message);
            }
            finally
            {
                TestContextProvider.Clear();
                instance.Dispose();
            }
        }

        /// <summary>
        /// Constructor initializing LightBDD scope.
        /// </summary>
        protected LightBddScope()
        {
            FixieFeatureCoordinator.InstallSelf(Configure());
            OnSetUp();
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

            configuration.Get<FeatureProgressNotifierConfiguration>()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.Get<ScenarioProgressNotifierConfiguration>()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format);

            OnConfigure(configuration);
            return configuration;
        }

        void IDisposable.Dispose()
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
    }
}