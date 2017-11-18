using Example.LightBDD.XUnit2;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Reporting.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.XUnit2;

/* This is a way to enable LightBDD - XUnit integration. 
 * It is possible to either use [assembly:LightBddScope] directly to use LightBDD with default configuration, 
 * or customize it in a way that is shown below.
 */
[assembly: ConfiguredLightBddScope]

namespace Example.LightBDD.XUnit2
{
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        /* This method allows to customize LightBDD behavior.
         * The code below configures LightBDD to produce also a plain text report after all tests are done.
         * More information on what can be customized can be found on wiki: https://github.com/LightBDD/LightBDD/wiki/LightBDD-Configuration#configurable-lightbdd-features 
         */
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ReportWritersConfiguration()
                .AddFileWriter<PlainTextReportFormatter>("~\\Reports\\{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_FeaturesReport.txt");
        }
    }
}