using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.Runner.Implementation;
using Xunit.Sdk;

namespace LightBDD.Runner
{
    /// <summary>
    /// </summary>
    [TestFrameworkDiscoverer($"LightBDD.Runner.Implementation.{nameof(LightBddFrameworkTypeDiscoverer)}", "LightBDD.Runner")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestFrameworkAttribute
    {
        internal LightBddConfiguration Configure()
        {
            var configuration = new LightBddConfiguration().WithFrameworkDefaults();

            configuration.ProgressNotifierConfiguration()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format);
            
            OnConfigure(configuration);
            return configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        protected virtual void OnConfigure(LightBddConfiguration configuration)
        {
        }
    }
}