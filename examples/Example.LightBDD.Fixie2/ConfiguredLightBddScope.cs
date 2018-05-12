using LightBDD.Core.Configuration;
using LightBDD.Fixie2;
using LightBDD.Framework.Reporting.Configuration;
using LightBDD.Framework.Reporting.Formatters;

/*
 * This is a way to enable LightBDD - Fixie integration.
 * It is required to do it in all assemblies with LightBDD scenarios.
 */

namespace Example.LightBDD.Fixie2
{
    /// <summary>
    /// This class is necessary to configure Fixie to recognize LightBDD tests
    /// </summary>
    internal class WithLightBddConventions : LightBddDiscoveryConvention
    {
        /// <summary>
        /// This constructor is required in order to pass category filters. Please also remember that <c>params</c> modifier is required here.
        /// </summary>
        public WithLightBddConventions(params string[] args)
            : base(args)
        {
        }
    }

    /// <summary>
    /// This class extends LightBddScope and allows to customize the default configuration of LightBDD.
    /// It is also possible here to override OnSetUp() and OnTearDown() methods to execute code that has to be run once, before or after all tests.
    /// </summary>
    internal class ConfiguredLightBddScope : LightBddScope
    {
        /// <summary>
        /// This method allows to customize LightBDD behavior.
        /// The code below configures LightBDD to produce also a plain text report after all tests are done.
        /// More information on what can be customized can be found on wiki: https://github.com/LightBDD/LightBDD/wiki/LightBDD-Configuration#configurable-lightbdd-features 
        /// </summary>
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ReportWritersConfiguration()
                .AddFileWriter<PlainTextReportFormatter>("~\\Reports\\{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_FeaturesReport.txt");
        }
    }
}