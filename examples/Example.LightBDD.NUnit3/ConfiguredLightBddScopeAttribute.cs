using Example.LightBDD.NUnit3;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.NUnit3;
using NUnit.Framework;

[assembly:Parallelizable(ParallelScope.Fixtures)]

/*
 * This is a way to enable LightBDD - NUnit integration.
 * It is required to do it in all assemblies with LightBDD scenarios.
 * It is possible to either use [assembly:LightBddScope] directly to use LightBDD with default configuration, 
 * or customize it in a way that is shown below.
 */
[assembly: ConfiguredLightBddScope]
namespace Example.LightBDD.NUnit3
{
    /// <summary>
    /// This class extends LightBddScopeAttribute and allows to customize the default configuration of LightBDD.
    /// It is also possible here to override OnSetUp() and OnTearDown() methods to execute code that has to be run once, before or after all tests.
    /// </summary>
    internal class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
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
                .AddFileWriter<XmlReportFormatter>("~\\Reports\\FeaturesReport.xml")
                .AddFileWriter<MarkdownReportFormatter>("~\\Reports\\FeaturesReport.md")
                .AddFileWriter<PlainTextReportFormatter>("~\\Reports\\{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_FeaturesReport.txt");
        }
    }
}