using LightBDD.Core.Configuration;
using LightBDD.Example.AcceptanceTests.XUnit2;
using LightBDD.Framework.Reporting.Configuration;
using LightBDD.Framework.Reporting.Formatters;
using LightBDD.XUnit2;

[assembly: ConfiguredLightBddScope]
namespace LightBDD.Example.AcceptanceTests.XUnit2
{
    public class ConfiguredLightBddScopeAttribute : LightBddScopeAttribute
    {
        protected override void OnConfigure(LightBddConfiguration configuration)
        {
            configuration
                .ReportWritersConfiguration()
                .AddFileWriter<PlainTextReportFormatter>("~\\Reports\\{TestDateTimeUtc:yyyy-MM-dd-HH_mm_ss}_FeaturesReport.txt");
        }
    }
}