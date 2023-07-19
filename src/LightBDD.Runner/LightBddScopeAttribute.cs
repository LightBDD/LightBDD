using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.XUnit2.Implementation;
using Xunit.Sdk;

namespace LightBDD.XUnit2
{
    /// <summary>
    /// </summary>
    [TestFrameworkDiscoverer($"LightBDD.XUnit2.Implementation.{nameof(LightBddFrameworkTypeDiscoverer)}", "LightBDD.Runner")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestFrameworkAttribute
    {
        internal void Configure(LightBddConfiguration configuration)
        {
            configuration.WithFrameworkDefaults();

            configuration.ProgressNotifierConfiguration()
                .AppendFrameworkDefaultProgressNotifiers();

            configuration.ExceptionHandlingConfiguration()
                .UpdateExceptionDetailsFormatter(new DefaultExceptionFormatter().WithTestFrameworkDefaults().Format);
            
            OnConfigure(configuration);
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