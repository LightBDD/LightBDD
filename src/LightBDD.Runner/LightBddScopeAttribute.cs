using System;
using LightBDD.Core.Configuration;
using LightBDD.Core.Formatting.ExceptionFormatting;
using LightBDD.Framework.Configuration;
using LightBDD.Runner.Configuration;
using LightBDD.Runner.Implementation;
using Xunit;
using Xunit.v3;

namespace LightBDD.Runner
{
    /// <summary>
    /// </summary>
    [TestFramework($"LightBDD.Runner.Implementation.{nameof(LightBddTestFramework)}", "LightBDD.Runner")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LightBddScopeAttribute : Attribute, ITestFrameworkAttribute
    {
        internal void Configure(LightBddConfiguration configuration)
        {
            configuration.WithFrameworkDefaults();

            configuration.ForMetadata()
                .RegisterEngineAssembly(typeof(LightBddScopeAttribute).Assembly);

            configuration.Services.ConfigureProgressNotifiers()
                .AddFrameworkDefaultProgressNotifiers();

            configuration.Services
                .ConfigureExceptionFormatter(x => x.Use(new DefaultExceptionFormatter().WithTestFrameworkDefaults()));

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