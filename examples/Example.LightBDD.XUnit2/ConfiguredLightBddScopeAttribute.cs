using Example.LightBDD.XUnit2;
using LightBDD.Core.Configuration;
using LightBDD.Framework.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.XUnit2;

/*
 * This is a way to enable LightBDD - XUnit integration.
 * It is required to do it in all assemblies with LightBDD scenarios.
 * It is possible to either use [assembly:LightBddScope] directly to use LightBDD with default configuration, 
 * or customize it in a way that is shown below.
 */
[assembly: ConfiguredLightBddScope]
/*
 * This is a LightBDD specific, experimental attribute enabling inter-class test parallelization
 * (as long as class does not implement IClassFixture<T> nor ICollectionFixture<T> attribute
 * nor have defined named collection [Collection] attribute)
 */
[assembly: ClassCollectionBehavior(AllowTestParallelization = true)]

namespace Example.LightBDD.XUnit2
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
                .AddFileWriter<PlainTextReportFormatter>("~\\Reports\\{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_FeaturesReport.txt");
        }
    }
}